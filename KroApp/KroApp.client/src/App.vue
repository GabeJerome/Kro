<template>
  <main>
    <div>
      <div>Welcome to Kro</div>
      <div class="card">
        <span>
          <InputText
            v-model="value"
            type="text"
          />
          <Button label="Save" />
        </span>
        <span>{{ text }}</span>
      </div>
    </div>
    <Button
      label="Toggle Dark Mode"
      @click="toggleDarkMode"
    />
  </main>
</template>

<script setup lang="ts">
import InputText from "primevue/inputtext";

import Button from "primevue/button";
import { ref } from "vue";
const text = ref("");

function toggleDarkMode() {
  document.documentElement.classList.toggle("dark-mode");
}
</script>

<style scoped></style>
