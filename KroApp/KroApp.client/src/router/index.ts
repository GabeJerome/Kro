import { createRouter, createWebHistory, type RouterOptions } from "vue-router";
import LoginRegister from "@/views/LoginRegister.vue";
import UserHome from "@/views/UserHome.vue";
import Landing from "@/views/Landing.vue";

const routes = [
  { path: "/user-home", name: "User Home", component: UserHome },
  { path: "", name: "Authenticate", component: LoginRegister },
];

const routerOptions: RouterOptions = {
  history: createWebHistory(),
  routes,
};
export const router = createRouter(routerOptions);
