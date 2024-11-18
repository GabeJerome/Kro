<template>
  <main class="tablet-width mobile-medium-width mobile-full-width">
    <DrawerMenu />
    <RouterView />
    <Toast />
  </main>
</template>

<script setup lang="ts">
import { onMounted } from "vue";
import { Toast } from "primevue";
import DrawerMenu from "@/Components/DrawerMenu.vue";

onMounted(() => {
  const darkModePreference = sessionStorage.getItem("dark-mode");
  if (darkModePreference === "enabled") {
    document.documentElement.classList.add("dark-mode");
  }
});
</script>

<style scoped></style>
