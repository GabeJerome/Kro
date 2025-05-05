<template>
  <Button
    label="Add Ingredients"
    @click="showSearch = !showSearch"
  />

  <div class="ingredient-window-container">
    <transition name="slide-fade">
      <div
        v-show="showSearch"
        id="ingredients-search"
        class="search-panel"
      >
        <IngredientsSearch />
      </div>
    </transition>

    <IngredientsDisplay
      id="ingredient-display"
      class="display-panel"
    />
  </div>
</template>

<script setup lang="ts">
import IngredientsDisplay from "@/Components/IngredientsDisplay.vue";
import IngredientsSearch from "@/Components/IngredientsSearch.vue";
import Button from "primevue/button";
import { ref } from "vue";

const showSearch = ref<boolean>(false);
</script>

<style scoped>
.ingredient-window-container {
  display: flex;
  flex-direction: row;
  gap: 0.5rem;
  width: 100%;
  height: 100%;
  overflow: hidden;
}

.search-panel {
  flex-grow: 1;
  flex-shrink: 0;
  flex-basis: 0;
  min-width: 300px;
  transition:
    flex-basis 0.4s ease-out,
    flex-grow 0.4s ease-out,
    min-width 0.4s ease-out;
}

.display-panel {
  flex-grow: 1;
  flex-basis: 0;
  min-width: 0;
  transition:
    flex-basis 0.4s ease-out,
    flex-grow 0.4s ease-out;
}

.slide-fade-enter-active {
  transition:
    transform 0.4s ease-out,
    opacity 0.25s ease-out 0.15s,
    flex-basis 0.4s ease-out,
    flex-grow 0.4s ease-out,
    min-width 0.4s ease-out;
}

.slide-fade-leave-active {
  transition:
    transform 0.3s ease-in,
    opacity 0.2s ease-in,
    flex-basis 0.3s ease-in,
    flex-grow 0.3s ease-in,
    min-width 0.3s ease-in;
}

.slide-fade-enter-from {
  transform: translateX(-100%);
  opacity: 0;
  flex-basis: 0;
  flex-grow: 0;
  min-width: 0;
  padding-left: 0;
  padding-right: 0;
  margin-left: 0;
  margin-right: 0;
  overflow: hidden;
}

.slide-fade-leave-to {
  transform: translateX(-100%);
  opacity: 0;
  flex-basis: 0;
  flex-grow: 0;
  min-width: 0;
  padding-left: 0;
  padding-right: 0;
  margin-left: 0;
  margin-right: 0;
  overflow: hidden;
}
</style>
