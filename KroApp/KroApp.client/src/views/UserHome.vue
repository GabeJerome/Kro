<template>
  <div class="page-container">
    <div class="title-bar">
      {{ username }}
    </div>

    <Tabs
      value="0"
      class="tabs-container"
      pt:root:class="tabs-root"
    >
      <TabList
        pt:root:class="my-tablist"
        pt:content:class="my-tab"
        pt:tablist:class="my-tab-buttons"
        :pt="{
          tablist: {
            style: {
              background: 'transparent',
            },
            class: {
              'p-tab-active': 'my-tab-active',
            },
          },
        }"
      >
        <Tab value="0"> <i class="fa-solid fa-pepper-hot" /> Ingredients </Tab>
        <Tab value="1"> <i class="fa-solid fa-book" /> Recipes</Tab>
        <Tab value="2"> <i class="fa-solid fa-list-check" /> Grocery List</Tab>
      </TabList>
      <TabPanels class="tab-panels">
        <TabPanel value="0">
          <IngredientWindow />
        </TabPanel>
        <TabPanel value="1"> This is the recipe window </TabPanel>
        <TabPanel value="2"> This is the grocery window </TabPanel>
      </TabPanels>
    </Tabs>
  </div>
</template>

<script setup lang="ts">
import { ref } from "vue";
import { onMounted } from "vue";
import Tabs from "primevue/tabs";
import TabList from "primevue/tablist";
import Tab from "primevue/tab";
import TabPanels from "primevue/tabpanels";
import TabPanel from "primevue/tabpanel";
import auth from "@/api/auth";
import IngredientWindow from "./IngredientWindow.vue";

const username = ref<string>("");

onMounted(() => {
  username.value = auth.getUsername(auth.getToken()!) || "";
});
</script>

<style scoped>
.page-container {
  display: flex;
  flex-direction: column;
  height: 100%;
}

.title-bar {
  font-size: 48px;
  margin-bottom: 1rem;
}

.tabs-container {
  display: flex;
  flex-grow: 1;
}

.tabs-container > * {
  border-radius: 12px;
}

.my-tablist {
  width: fit-content;
}

.my-tab-buttons > button {
  border-radius: 8px 8px 0px 0px;
  background-color: var(--secondary-bg);
}

.my-tab-buttons > .p-tab-active {
  background: linear-gradient(
    to bottom,
    var(--third-bg) 40%,
    var(--secondary-bg) 100%
  );
  border-color: var(--p-tabs-tab-active-border-color);
  color: var(--p-tabs-tab-active-color);
  border-top-width: 3px;
}

.tab-panels {
  flex-grow: 1;
  border-top-left-radius: 0;
  background-color: var(--secondary-bg);
}
</style>
