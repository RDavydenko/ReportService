<template>
  <tr>
    <td>
      <div v-if="loading">
        Loading...
      </div>
      <span v-else>
        {{ user.email }}
      </span>
    </td>
    <td>{{ user.name }}</td>
    <td>{{ user.surname }}</td>
    <td>{{ user.patronymic }}</td>
    <td>
      <div class="actions">
        <button
          type="button"
          class="btn btn-small blue"
          @click="details(user.id)"
        >
          Подробнее
        </button>
      </div>
    </td>
  </tr>
</template>

<script>
export default {
  props: ["userId"],
  data() {
    return {
      user: {},
      loading: true
    };
  },
  async created() {
    let response = await fetch(
      "https://localhost:44375/api/users/" + this.userId.id
    );
    let json = await response.json();
    if (json.ok) {
      this.user = json.object;
      this.loading = false;
    }
  },
  methods: {
    details(id) {
      this.$emit("open-modal", id);
    }
  }
};
</script>

<style scoped>
.user-wrap {
  padding: 3px;
  border: 1px solid #ccc;
  border-radius: 5px;
  width: 100%;
  min-height: 50px;
}
.user-wrap:hover {
  background-color: #ccc;
}
.user-about {
  display: flex;
  justify-content: left;
  font-size: 16px;
}
span {
  margin-right: 25px;
}
</style>