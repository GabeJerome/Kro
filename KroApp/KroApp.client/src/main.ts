import "./assets/main.css";

import { createApp } from "vue";
import App from "./App.vue";
import PrimeVue from "primevue/config";
import Lara from "@primevue/themes/lara";
import { definePreset } from "@primevue/themes";
import ToastService from "primevue/toastservice";
import router from "@/router";
import Tooltip from "primevue/tooltip";

const app = createApp(App);

const customVioletLight = {
  50: "{violet.50}",
  100: "{violet.100}",
  200: "{violet.200}",
  300: "{violet.300}",
  400: "{violet.400}",
  500: "{violet.500}",
  600: "{violet.600}",
  700: "{violet.700}",
  800: "{violet.800}",
  900: "{violet.900}",
  950: "{violet.950}",
};

const customVioletDark = {
  50: "#F8F3FF",
  100: "#EDE4FF",
  200: "#D6C7FF",
  300: "#BEA6F5",
  400: "#A48AE2",
  500: "#8E6EDB",
  600: "#7D56D8",
  700: "#744FCC",
  800: "#6F50CF",
  900: "#6546C5",
  950: "#573BB4",
};
const MyPreset = definePreset(Lara, {
  semantic: {
    primary: customVioletLight,
    colorScheme: {
      light: {
        formField: {
          background: customVioletLight[50],
          hoverBorderColor: "{surface.color}",
        },
        surface: customVioletLight,
        text: {
          color: customVioletLight[500],
        },
      },
      dark: {
        formField: {
          background: customVioletDark[800],
          hoverBorderColor: "{surface.color}",
        },
        surface: customVioletDark,
        text: {
          color: customVioletDark[200],
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
