<template>
  <div class="page-container">
    <div class="title-bar">
      {{ username }}
    </div>
    <TabMenu :model="items" />
    <div id="ingredient-window">
      <IngredientWindow />
    </div>
    <div
      id="recipe-window"
      style="display: none"
    >
      This is the recipe window
    </div>
    <div
      id="grocery-window"
      style="display: none"
    >
      This is the grocery window
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from "vue";
import { TabMenu } from "primevue";
import { onMounted } from "vue";
import auth from "@/api/auth";
import IngredientWindow from "./IngredientWindow.vue";

const username = ref<string>("");

const items = ref([
  {
    label: "Ingredients",
    icon: "fa-solid fa-pepper-hot",
    command: switchToIngredients,
  },
  { label: "Recipes", icon: "fa-solid fa-book", command: switchToRecipes },
  {
    label: "Grocery List",
    icon: "fa-solid fa-list-check",
    command: switchToGroceries,
  },
]);

onMounted(() => {
  username.value = auth.getUsername(auth.getToken()!) || "";
});

function switchToIngredients() {
  document.getElementById("recipe-window")!.style.display = "none";
  document.getElementById("grocery-window")!.style.display = "none";

  document.getElementById("ingredient-window")!.style.display = "block";
}
function switchToRecipes() {
  document.getElementById("ingredient-window")!.style.display = "none";
  document.getElementById("grocery-window")!.style.display = "none";

  document.getElementById("recipe-window")!.style.display = "block";
}
function switchToGroceries() {
  document.getElementById("ingredient-window")!.style.display = "none";
  document.getElementById("recipe-window")!.style.display = "none";

  document.getElementById("grocery-window")!.style.display = "block";
}
</script>

<style scoped>
.page-container {
  display: flex;
  flex-direction: column;
}

.title-bar {
  font-size: 48px;
  margin-bottom: 1rem;
}
</style>
