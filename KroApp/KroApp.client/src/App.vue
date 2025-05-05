<template>
  <main>
    <DrawerMenu />
    <div class="page-content">
      <RouterView />
    </div>
    <Toast />
  </main>
</template>

<script setup lang="ts">
import { onMounted } from "vue";
import { Toast } from "primevue";
import DrawerMenu from "@/Components/DrawerMenu.vue";

onMounted(() => {
  const darkModePreference = localStorage.getItem("dark-mode");
  if (darkModePreference === "enabled") {
    document.documentElement.classList.add("dark-mode");
  }
});
</script>

<style scoped>
.page-content {
  height: 100%;
}
</style>
