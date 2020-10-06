<template>
  <div>
    <div class="menu">
      <button class="btn green" @click="addReport">
        Добавить<i class="left material-icons">add</i>
      </button>
    </div>

    <!-- Modal Structure -->
    <ReportDetails
      v-if="modal.modalShow"
      :reportId="modal.currentModalReport"
      :mode="modal.mode"
      @close-modal="closeModal"
    ></ReportDetails>

    <table class="highlight">
      <thead>
        <tr>
          <th>Remark</th>
          <th>Date</th>
          <th>Hours</th>
          <th>Owner</th>
          <th>Actions</th>
        </tr>
      </thead>

      <tbody>
        <Report
          v-for="report in reportIdx"
          :key="report.id"
          :reportId="report.id"
          @open-modal="openModal"
        />
      </tbody>
    </table>
  </div>
</template>

<script>
import Report from "@/components/Report";
import ReportDetails from "@/components/ReportDetails";

export default {
  data() {
    return {
      reportIdx: [],
      modal: {
        modalShow: false,
        currentModalReport: 0,
        mode: 'change'
      }
    };
  },
  
  async created() {
    let response = await fetch("https://localhost:44375/api/reports")
    let json = await response.json()
    if (json.ok) {
      this.reportIdx = json.object
    }
  },
  
  methods: {
    openModal(id) {
      this.modal.mode = 'change'
      this.modal.currentModalReport = id
      this.modal.modalShow = true
    },
    closeModal() {
      this.modal.modalShow = false
    },
    addReport() {
      this.modal.mode = 'create'
      this.modal.currentModalReport = 0
      this.modal.modalShow = true
    }
  },
  components: {
    Report,
    ReportDetails
  }
};
</script>

<style scoped>
.menu {
  float: right;
  margin-top: 20px;
}
</style>