export default {
	state: {
		users: []
	},
	getters: {
		users(state) {
			return state.users;
		}
	},
	mutations: {
		updateUsers(state, users) {
			state.users = users;
		},
		updateUser(state, user) {
			for (let i = 0; i < state.users.length; i++) {
				if (state.users[i].id === user.id) {
					state.users[i] = user;
					break;
				}
			}
		},
		addUser(state, user) {
			state.users.push(user);
		},
		removeUser(state, userId) {
			let idx = 0;
			for (let i = 0; i < state.users.length; i++) {
				if (state.users[i].id === userId) {
					idx = i;
					break;
				}
			}
			if (idx !== 0) {
				state.users.splice(idx, 1);
			}
		}
	},
	actions: {
		async fetchUsers(context) {
			let response = await fetch("https://localhost:44375/api/users");
			let users = await response.json();
			if (response.ok) {
				context.commit("updateUsers", users.object)
			}
			return users;
		},
		async updateUser(context, user) {
			let response = await fetch(
				"https://localhost:44375/api/users/" + user.id + "/edit",
				{
					method: "POST",
					headers: {
						"Content-Type": "application/json;charset=utf-8"
					},
					body: JSON.stringify(user)
				}
			)
			let json = await response.json();
			if (response.ok) {
				if (json.ok) {
					context.commit('updateUser', json.object)
				}
			}
			return json;
		},
		async createUser(context, user) {
			let response = await fetch("https://localhost:44375/api/users/add", {
				method: "POST",
				headers: {
					"Content-Type": "application/json;charset=utf-8"
				},
				body: JSON.stringify(user)
			})
			let json = await response.json();
			if (response.ok) {
				if (json.ok) {
					context.commit('addUser', json.object)
				}
			}
			return json;
		},
		async removeUser(context, userId) {
			let response = await fetch(
				"https://localhost:44375/api/users/" + userId + "/delete",
				{
					method: "POST",
					headers: {
						"Content-Type": "application/json;charset=utf-8"
					}
				}
			);
			let json = await response.json();
			if (json.ok) {
				context.commit("removeUser", userId);
			}
		}
	}
}