<template>
  <main class="tablet-width mobile-medium-width mobile-full-width">
    <LoginRegister />
    <Button
      label="Toggle Dark Mode"
      @click="toggleDarkMode"
    />
  </main>
</template>

<script setup lang="ts">
import Button from "primevue/button";
import LoginRegister from "@/views/LoginRegister.vue";
import { onMounted } from "vue";

function toggleDarkMode() {
  const isDarkMode = document.documentElement.classList.toggle("dark-mode");
  sessionStorage.setItem("dark-mode", isDarkMode ? "enabled" : "disabled");
}

onMounted(() => {
  const darkModePreference = sessionStorage.getItem("dark-mode");
  if (darkModePreference === "enabled") {
    document.documentElement.classList.add("dark-mode");
  }
});
</script>

<style scoped></style>
