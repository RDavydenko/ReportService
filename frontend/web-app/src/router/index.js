import Vue from "vue";
import VueRouter from "vue-router";
import Users from "../views/Users.vue";
import Home from "../views/Home.vue"

Vue.use(VueRouter);

const routes = [
  {
    path: "/",
    name: "Home",
    component: Home
  },
  {
    path: "/users",
    name: "Users",
    component: Users
  },
  {
    path: "/reports",
    name: "Reports",
    component: () =>
      import("../views/Reports.vue")
  }
];

const router = new VueRouter({
  mode: "history",
  base: process.env.BASE_URL,
  routes
});

export default router;
