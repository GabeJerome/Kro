<template>
  <div class="card">
    <FloatLabel variant="on">
      <InputText
        id="search-input"
        v-model="searchString"
        type="text"
        name="search-input"
        class="p-mb-3"
      />
      <label for="search-input"> Search </label>
    </FloatLabel>
    <Button
      label="List Ingredients"
      name="list-ingredients"
      @click="submitSearch"
    />
  </div>
</template>

<script setup lang="ts">
import { Button, InputText, FloatLabel } from "primevue";
import { listIngredients } from "@/api/ingredients";
import { ref } from "vue";

const searchString = ref<string>("");

const submitSearch = async () => {
  try {
    console.log("Searching for ingredients with:", searchString.value);
    const response = await listIngredients(searchString.value);
    console.log(response);
  } catch (error) {
    console.error("Error fetching ingredients:", error);
  }
};
</script>
