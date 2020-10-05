<template>
  <tr class="report-wrap">
    <td class="report-remark">{{ report.remark }}</td>
    <td class="report-date">
      {{ new Date(report.date).toLocaleDateString() }}
    </td>
    <td class="report-hours">{{ report.hours }}</td>
    <td class="report-owner">{{ owner.surname }} {{ owner.name }}</td>
    <td>
      <div class="actions">
        <button
          type="button"
          class="btn btn-small blue"
          @click="details(report.id)"
        >
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
  props: ["reportId"],
  data() {
    return {
      report: {},
      owner: {}
    };
  },
  async created() {
    let response = await fetch(
      "https://localhost:44375/api/reports/" + this.reportId
    );
    let json = await response.json();
    if (json.ok) {
      this.report = json.object;
    }

    let responseUser = await fetch(
      "https://localhost:44375/api/users/" + this.report.userId
    );
    let jsonUser = await responseUser.json();
    if (jsonUser.ok) {
      this.owner = jsonUser.object;
    }
  },
  methods: {
    details(id) {
      this.$emit("open-modal", id);
	 },
	 async remove() {
		 let response = await fetch("https://localhost:44375/api/reports/" + this.report.id + "/delete", {
			 method: "POST",
			 headers: {
            "Content-Type": "application/json;charset=utf-8"
          }
		 })
		 let json = await response.json();
		 if (json.ok) {
			 this.$emit('remove', this.report.id);
		 }
	 }
  }
};
</script>

<style>
</style>