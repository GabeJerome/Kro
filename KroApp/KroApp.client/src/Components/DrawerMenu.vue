<template>
  <Drawer
    v-model:visible="visible"
    class="menu"
    :modal="true"
    :dismissible="true"
  >
    <div class="drawer-content">
      <Button
        label="Logout"
        icon="pi pi-sign-out"
        @click="logout"
      />
      <Button
        :label="isDarkMode ? 'Light Mode' : 'Dark Mode'"
        :icon="`${isDarkMode ? 'pi pi-sun' : 'pi pi-moon'}`"
        @click="toggleDarkMode"
      />
    </div>
  </Drawer>

  <Button
    icon="pi pi-bars"
    class="menu-toggle-btn"
    @click="toggleDrawer"
  />
</template>

<script setup lang="ts">
import { ref } from "vue";
import Drawer from "primevue/drawer";
import Button from "primevue/button";
import auth from "@/api/auth";

const visible = ref(false);
const isDarkMode = ref(
  document.documentElement.classList.contains("dark-mode"),
);

function toggleDrawer() {
  visible.value = !visible.value;
}

function logout() {
  auth.logout();
  toggleDrawer();
}

function toggleDarkMode() {
  document.documentElement.classList.toggle("dark-mode");
  isDarkMode.value = !isDarkMode.value;
  localStorage.setItem("dark-mode", isDarkMode.value ? "enabled" : "disabled");
}
</script>

<style scoped>
.menu {
  width: 250px;
  background-color: var(--p-surface-200);
}

.drawer-content {
  display: flex;
  flex-direction: column;
  gap: 1rem;
  padding: 1.5rem;
}

.menu-link {
  color: var(--p-text-color);
  text-decoration: none;
  font-weight: 500;
  transition: color 0.3s ease;
}

.menu-link:hover {
  color: var(--p-primary-700);
}

.menu-toggle-btn {
  position: fixed;
  top: 1rem;
  left: 1rem;
  background-color: var(--p-primary-600);
  color: white;
}
</style>
