<template>
  <tr>
    <td>
      {{ user.email }}
    </td>
    <td>{{ user.name }}</td>
    <td>{{ user.surname }}</td>
    <td>{{ user.patronymic }}</td>
    <td>
      <div class="actions">
        <button type="button" class="btn btn-small blue" @click="details">
          Подробнее
        </button>
        <button @click="remove" class="btn btn-small pink">
          <i class="material-icons">delete</i>
        </button>
      </div>
    </td>
  </tr>
</template>

<script>
export default {
  props: ["user"],
  methods: {
    details() {
      this.$emit("open-modal", this.user.id);
    },
    async remove() {
      this.$store.dispatch("removeUser", this.user.id);
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