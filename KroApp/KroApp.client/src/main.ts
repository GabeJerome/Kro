import "./assets/main.css";

import { createApp } from "vue";
import App from "./App.vue";
import PrimeVue from "primevue/config";
import Aura from "@primevue/themes/aura";
import { definePreset } from "@primevue/themes";
import ToastService from "primevue/toastservice";
import router from "@/router";
import Tooltip from "primevue/tooltip";
import "@/assets/colors";
import { darkTheme, lightTheme } from "@/assets/colors";

const app = createApp(App);

const MyPreset = definePreset(Aura, {
  semantic: {
    colorScheme: {
      light: {
        surface: lightTheme.surface,
        primary: lightTheme.primary,
        accent: lightTheme.accent,
      },
      dark: {
        surface: darkTheme.surface,
        primary: darkTheme.primary,
        accent: darkTheme.accent,
      },
    },
  },
  components: {
    card: {
      root: {
        borderRadius: "0.5rem",
      },
      title: {
        fontSize: "1.5rem",
      },
      colorScheme: {
        light: {
          root: {
            background: "var(--background-secondary)",
          },
        },
        dark: {
          root: {
            background: "var(--background-secondary)",
          },
        },
      },
    },
  },
});

app.use(PrimeVue, {
  theme: {
    preset: MyPreset,
    options: {
      prefix: "p",
      darkModeSelector: ".dark-mode",
      cssLayer: false,
    },
  },
});

app.use(ToastService);
app.use(router);

app.directive("tooltip", Tooltip);

app.mount("#app");
