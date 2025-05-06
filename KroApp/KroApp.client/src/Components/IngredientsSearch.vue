<template>
  <div class="ingredient-search-container">
    <h2>Ingredient Search</h2>

    <div class="search-container">
      <InputText
        v-model="searchString"
        placeholder="Search Ingredients"
        class="search-input"
        @keyup.enter="submitSearch"
      />
      <Button
        label="Search"
        icon="pi pi-search"
        :loading="loading"
        @click="submitSearch"
      />
    </div>

    <div
      v-if="loading"
      class="loading-container"
    >
      <Skeleton
        width="30%"
        height="2rem"
      ></Skeleton>
      <Skeleton
        width="100%"
        height="2rem"
      ></Skeleton>
      <Skeleton
        width="100%"
        style="flex-grow: 1"
      ></Skeleton>
      <Skeleton
        width="100%"
        height="56px"
      ></Skeleton>
    </div>

    <div
      v-if="error"
      class="p-message p-message-error message-container"
    >
      <span class="p-message-icon pi pi-times-circle" />
      <span class="p-message-text">Error loading ingredients: {{ error }}</span>
    </div>

    <div
      v-if="ingredients && ingredients.length > 0 && !loading"
      class="results-container"
    >
      <h3>Search Results for "{{ lastSearchTerm }}"</h3>
      <DataTable
        :value="ingredients"
        paginator
        :rows="10"
        :rows-per-page-options="[5, 10, 25, 50]"
        removable-sort
        striped-rows
        size="small"
        scrollable
        scrollHeight="flex"
      >
        <template #empty> No ingredients found. </template>

        <Column
          field="name"
          header="Name"
          sortable
          filter
          filter-placeholder="Filter by name"
        />
        <Column
          field="brand"
          header="Brand"
          sortable
          filter
          filter-placeholder="Filter by brand"
        />
        <Column
          field="packageWeight"
          header="Package Weight"
        />
        <Column header="Ingredients List">
          <template #body="slotProps">
            <span
              v-if="
                slotProps.data.ingredients &&
                slotProps.data.ingredients.length > 0
              "
            >
              {{ slotProps.data.ingredients.slice(0, 2).join(", ") }}
              {{ slotProps.data.ingredients.length > 2 ? "..." : "" }}
            </span>
            <span v-else>N/A</span>
          </template>
        </Column>
      </DataTable>
    </div>

    <div
      v-else-if="
        !loading &&
        !error &&
        searched &&
        (!ingredients || ingredients.length === 0)
      "
      class="p-message p-message-info message-container"
    >
      <span class="p-message-icon pi pi-info-circle" />
      <span class="p-message-text"
        >No ingredients found matching "{{ lastSearchTerm }}".</span
      >
    </div>
  </div>
</template>

<script setup>
import { ref } from "vue";

import DataTable from "primevue/datatable";
import Column from "primevue/column";
import InputText from "primevue/inputtext";
import Button from "primevue/button";
import Skeleton from "primevue/skeleton";
import { listIngredients } from "@/api/ingredients";

const searchString = ref("");
const ingredients = ref(null);
const loading = ref(false);
const error = ref(null);
const searched = ref(false);
const lastSearchTerm = ref("");

const submitSearch = async () => {
  if (!searchString.value?.trim()) {
    ingredients.value = [];
    searched.value = false;
    return;
  }

  loading.value = true;
  error.value = null;
  ingredients.value = null;
  searched.value = true;
  lastSearchTerm.value = searchString.value;

  try {
    const response = await listIngredients(searchString.value);

    ingredients.value = Array.isArray(response) ? response : [];
  } catch (error_) {
    console.error("Error fetching ingredients:", error_);
    error.value =
      error_.message || "An unknown error occurred while fetching data.";
    ingredients.value = [];
  } finally {
    loading.value = false;
  }
};
</script>

<style scoped>
.ingredient-search-container {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
  height: 100%;
}

.ingredient-search-container > h2,
.search-container,
.message-container,
.results-title {
  flex-shrink: 0;
}

.search-container {
  display: flex;
  gap: 0.5rem;
}

.search-input {
  flex-grow: 1;
}

.loading-container {
  flex-grow: 1;
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.message-container > span {
  margin-right: 0.5rem;
}

.results-container {
  flex-grow: 1;
  display: flex;
  flex-direction: column;
  min-height: 0;
  overflow: hidden;
}

.results-container > :deep(.p-datatable) {
  min-height: 0;
}
</style>
