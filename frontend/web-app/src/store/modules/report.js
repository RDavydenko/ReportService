export default {
	state: {
		reports: []
	},
	getters: {
		reports(state) {
			return state.reports;
		},
		getReportById: (state) => (id) => {
			return state.reports.find(x => x.id === id);
		}
	},
	mutations: {
		updateReports(state, reports) {
			state.reports = reports;
		},
		updateReport(state, report) {
			for (let i = 0; i < state.reports.length; i++) {
				if (state.reports[i].id === report.id) {
					state.reports[i] = report;
					break;
				}
			}
		},
		addReport(state, report) {
			state.reports.push(report);
		},
		removeReport(state, reportId) {
			let idx = 0;
			for (let i = 0; i < state.reports.length; i++) {
				if (state.reports[i].id === reportId) {
					idx = i;
					break;
				}
			}
			if (idx !== 0) {
				state.reports.splice(idx, 1);
			}
		}
	},
	actions: {
		async fetchReports(context) {
			let response = await fetch("https://localhost:44375/api/reports");
			let reports = await response.json();
			if (response.ok) {
				context.commit("updateReports", reports.object)
			}
			return reports;
		},
		async updateReport(context, report) {
			let response = await fetch(
				"https://localhost:44375/api/reports/" + report.id + "/edit",
				{
					method: "POST",
					headers: {
						"Content-Type": "application/json;charset=utf-8"
					},
					body: JSON.stringify(report)
				}
			)
			let json = await response.json();
			if (response.ok) {
				if (json.ok) {
					context.commit('updateReport', json.object)
				}
			}
			return json;
		},
		async createReport(context, report) {
			let response = await fetch("https://localhost:44375/api/reports/add", {
				method: "POST",
				headers: {
					"Content-Type": "application/json;charset=utf-8"
				},
				body: JSON.stringify(report)
			})
			let json = await response.json();
			if (response.ok) {
				if (json.ok) {
					context.commit('addReport', json.object)
				}
			}
			return json;
		},
		async removeReport(context, reportId) {			
			let response = await fetch(
				"https://localhost:44375/api/reports/" + reportId + "/delete",
				{
				  method: "POST",
				  headers: {
					 "Content-Type": "application/json;charset=utf-8"
				  }
				}
			 );
			 let json = await response.json();			
			 if (json.ok) {
				context.commit("removeReport", reportId);
			 }
		}
	}
}