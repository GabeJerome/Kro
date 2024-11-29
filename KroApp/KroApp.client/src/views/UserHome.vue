<template>
  <div class="page-container">
    <div class="title-bar">{{ username }}</div>
    <TabMenu :model="items" />
  </div>
  <div id="ingredient-list">This is the ingredient list</div>
  <div
    id="recipe-list"
    style="display: none"
  >
    This is the recipe list
  </div>
  <div
    id="grocery-list"
    style="display: none"
  >
    This is the grocery list
  </div>
</template>

<script setup lang="ts">
import { ref } from "vue";
import { TabMenu } from "primevue";
import { onMounted } from "vue";
import auth from "@/api/auth";

const username = ref("");

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
  username.value = auth.getUsername(auth.getToken());
});

function switchToIngredients() {
  document.getElementById("recipe-list").style.display = "none";
  document.getElementById("grocery-list").style.display = "none";

  document.getElementById("ingredient-list").style.display = "block";
}
function switchToRecipes() {
  document.getElementById("ingredient-list").style.display = "none";
  document.getElementById("grocery-list").style.display = "none";

  document.getElementById("recipe-list").style.display = "block";
}
function switchToGroceries() {
  document.getElementById("ingredient-list").style.display = "none";
  document.getElementById("recipe-list").style.display = "none";

  document.getElementById("grocery-list").style.display = "block";
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
