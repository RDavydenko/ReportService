<template>
  <div class="modal" style="display: block">
    <Message v-if="message.display" :text="message.text" :type="message.type" />
    <div class="modal-content">
      <h3>{{ title }}</h3>
      <form>
        <div class="input-field col">
          <input v-model="report.remark" placeholder="Remark" type="text" />
        </div>
        <div class="input-field col">
          <input @change="dateChanged" :value="date" type="date" />
        </div>
        <div class="input-field col">
          <input v-model.number="report.hours" placeholder="0" type="number" />
        </div>

        <label>Пользователь (владелец)</label>
        <div>
          <select
            @focus="downloadUsers"
            v-model="report.userId"
            class="browser-default"
          >
            <option value="" disabled selected>Выберите пользователя</option>
            <option v-for="(user, i) in users" :key="i" :value="user.id"
              >{{ user.surname }} {{ user.name }}</option
            >
          </select>
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
  props: ["reportId", "mode"],
  data() {
    return {
      report: {},
      users: [],
      message: {
        display: false,
        text: "",
        type: ""
      }
    };
  },
  async created() {
    if (this.mode === "change") {
      this.report = this.$store.getters.getReportById(this.reportId);
    }
    this.$store.dispatch('fetchUsers');
  },
  computed: {
    date() {
      let date = new Date(this.report.date);
      let month = (date.getMonth() + 1 < 10 ? "0" : "") + (date.getMonth() + 1);
      let day = (date.getDate() < 10 ? "0" : "") + date.getDate();
      return `${date.getFullYear()}-${month}-${day}`;
    },
    title() {
      if (this.mode === "change") {
        return "Отчет " + this.reportId;
      }
      if (this.mode === "create") {
        return "Создание отчета";
      }
      return "";
    }
  },
  methods: {
    async saveChanges() {
      let response;
      if (this.mode === "change") {
        response = await this.$store.dispatch("updateReport", this.report);
      }
      if (this.mode === "create") {
        response = await this.$store.dispatch("createReport", this.report);
      }

      this.message.display = true;
      this.message.text = response.description;
      if (response.ok) {
        this.message.type = "success";
        this.report = response.object;
      } else {
        this.message.type = "error";
      }
    },
    dateChanged(e) {
      this.report.date = new Date(e.target.value);
    },
    async downloadUsers() {
      this.users = this.$store.getters.users;
    }
  },
  components: {
    Message
  }
};
</script>

<style>
</style>