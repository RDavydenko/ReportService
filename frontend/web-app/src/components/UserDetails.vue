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
      let response;
      if (this.mode === "change") {
        response = await fetch(
          "https://localhost:44375/api/users/" + this.user.id + "/edit",
          {
            method: "POST",
            headers: {
              "Content-Type": "application/json;charset=utf-8"
            },
            body: JSON.stringify(this.user)
          }
        );
      }
      
      if (this.mode === "create") {
        response = await fetch("https://localhost:44375/api/users/add", {
          method: "POST",
          headers: {
            "Content-Type": "application/json;charset=utf-8"
          },
          body: JSON.stringify(this.user)
        });
      }

      this.message.display = true;
      if (response.ok) {
        let json = await response.json();
        this.message.text = json.description;
        if (json.ok) {
          this.message.type = "success";
          this.user = json.object;
        } else {
          this.message.type = "error";
        }
      } else {
        this.message.text = "Ошибка!";
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