<template>
  <main>
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

function toggleDarkMode() {
  document.documentElement.classList.toggle("dark-mode");
}
</script>

<style scoped></style>
