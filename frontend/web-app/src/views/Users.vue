<template>
  <div>
    <div class="menu">
      <button class="btn green" @click="addUser">
        Добавить<i class="left material-icons">add</i>
      </button>
    </div>

    <!-- Modal Structure -->
    <UserDetails
      v-if="modal.modalShow"
      :userId="modal.currentModalUser"
      :mode="modal.mode"
      @close-modal="closeModal"
    ></UserDetails>

    <table class="highlight">
      <thead>
        <tr>
          <th>Email</th>
          <th>Name</th>
          <th>Surname</th>
          <th>Patronimyc</th>
          <th>Actions</th>
        </tr>
      </thead>
      <tbody>
        <User
          v-for="(user, index) in users"
          :key="index"
          :user="user"
          @open-modal="openModal"
        />
      </tbody>
    </table>
  </div>
</template>

<script>
import User from "@/components/User";
import UserDetails from "@/components/UserDetails";

export default {
  data() {
    return {
      modal: {
        modalShow: false,
        currentModalUser: 0,
        mode: "change"
      }
    };
  },
  computed: {
    users() {
      return this.$store.getters.users;
    }
  },
  async mounted() {
    this.$store.dispatch("fetchUsers");
  },
  methods: {
    openModal(id) {
      this.modal.mode = 'change'
      this.modal.currentModalUser = id;
      this.modal.modalShow = true;
    },
    closeModal() {
      this.modal.modalShow = false;
    },
    addUser() {
      this.modal.mode = 'create'
      this.modal.currentModalUser = 0
      this.modal.modalShow = true
    }
  },
  components: {
    User,
    UserDetails
  }
};
</script>

<style scoped>
.menu {
    float: right;
    margin-top: 20px;
  }
</style>