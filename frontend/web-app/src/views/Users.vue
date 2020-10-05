<template>
  <div>
    <!-- Modal Structure -->
    <UserDetails
      v-if="modal.modalShow"
      :userId="modal.currentModalUser"
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
          v-for="(user, index) in userIdx"
          :key="index"
          :userId="user"
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
      userIdx: [],
      modal: {
        modalShow: false,
        currentModalUser: 0
      }
    };
  },
  async mounted() {
    let response = await fetch("https://localhost:44375/api/users");
    let json = await response.json();
    if (json.ok) {
      this.userIdx = json.object;
    } else {
      // TODO: добавить сообщение об ошибке
    }
  },
  methods: {
    openModal(id) {
      this.modal.currentModalUser = id;
      this.modal.modalShow = true;
    },
    closeModal() {
      this.modal.modalShow = false;
    }
  },
  components: {
    User,
    UserDetails
  }
};
</script>

<style scoped>
</style>