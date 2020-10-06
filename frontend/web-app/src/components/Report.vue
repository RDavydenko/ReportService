<template>
  <tr class="report-wrap">
    <td class="report-remark">{{ report.remark }}</td>
    <td class="report-date">
      {{ new Date(report.date).toLocaleDateString() }}
    </td>
    <td class="report-hours">{{ report.hours }}</td>
    <td class="report-owner">{{ report.userSurname }} {{ report.userName }}</td>
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
  props: ["report"],
  data() {
    return {
      
    };
  },
  methods: {
    details() {
      this.$emit("open-modal", this.report.id);
    },
    async remove() {
      this.$store.dispatch("removeReport", this.report.id)
    }
  }
};
</script>

<style>
</style>