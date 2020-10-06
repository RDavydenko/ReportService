<template>
  <div class="modal" style="display: block">
    <Message v-if="message.display" :text="message.text" :type="message.type" />
    <div class="modal-content">
      <h3>Пользователь {{ user.id }}</h3>
      <form>
        <div class="input-field col">
          <input v-model="user.email" placeholder="Email" type="text" />
        </div>
        <div class="input-field col">
          <input v-model="user.name" placeholder="Имя" type="text" />
        </div>
        <div class="input-field col">
          <input v-model="user.surname" placeholder="Фамилия" type="text" />
        </div>
        <div class="input-field col">
          <input v-model="user.patronymic" placeholder="Отчество" type="text" />
        </div>
      </form>
    </div>
    <div class="modal-footer">
      <button
        class="btn modal-close waves-effect waves-light btn-flat"
        @click="$emit('close-modal')"
      >
        Отмена
      </button>
      <button
        class="btn modal-close waves-effect waves-green btn-flat"
        @click="saveChanges"
      >
        {{ mode === "change" ? "Изменить" : "Создать" }}
      </button>
    </div>
  </div>
</template>

<script>
import Message from "@/components/Message";

export default {
  props: ["userId", "mode"],
  data() {
    return {
      user: {},
      message: {
        display: false,
        text: "",
        type: ""
      }
    };
  },
  async created() {
    let response = await fetch(
      "https://localhost:44375/api/users/" + this.userId
    );
    let json = await response.json();
    if (json.ok) {
      this.user = json.object;
    }
  },
  methods: {
    async saveChanges() {
      let answer;
      if (this.mode === "change") {
        answer = await this.$store.dispatch("updateUser", this.user);
      }
      if (this.mode === "create") {
        answer = await this.$store.dispatch("createUser", this.user);
      }

      this.message.display = true;
      if (answer.ok) {
        this.message.text = answer.description;
        this.message.type = "success";
      } else {
        if (this.message.text.length == 0) {
          this.message.text = "Ошибка!"
        }
        this.message.type = "error";
      }
    }
  },
  components: {
    Message
  }
};
</script>

<style scoped>
</style>