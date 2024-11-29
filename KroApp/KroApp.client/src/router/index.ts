import { createRouter, createWebHistory, type RouterOptions } from "vue-router";
import LoginRegister from "@/views/LoginRegister.vue";
import UserHome from "@/views/UserHome.vue";
import auth from "@/api/auth";

const routes = [
  {
    path: "/user-home",
    name: "User Home",
    component: UserHome,
    meta: { requiresAuth: true },
  },
  { path: "", name: "Authenticate", component: LoginRegister },
];

const routerOptions: RouterOptions = {
  history: createWebHistory(),
  routes,
};

const router = createRouter(routerOptions);

router.beforeEach((to, from, next) => {
  const isAuthenticated = auth.isAuthenticated();
  if (to.meta.requiresAuth && !isAuthenticated) {
    next({ name: "Authenticate" });
  } else {
    next();
  }
});

export default router;
