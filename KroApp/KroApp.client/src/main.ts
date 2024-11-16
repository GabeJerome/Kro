import "./assets/main.css";

import { createApp } from "vue";
import App from "./App.vue";
import PrimeVue from "primevue/config";
import Lara from "@primevue/themes/lara";
import { definePreset } from "@primevue/themes";
import ToastService from "primevue/toastservice";

const app = createApp(App);

const customViolet = {
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

const MyPreset = definePreset(Lara, {
  semantic: {
    primary: customViolet,
    colorScheme: {
      light: {
        formField: {
          hoverBorderColor: "{surface.color}",
        },
        surface: customViolet,
      },
      dark: {
        formField: {
          hoverBorderColor: "{surface.color}",
        },
        surface: {
          50: "{purple.50}",
          100: "{purple.100}",
          200: "{purple.200}",
          300: "{purple.300}",
          400: "{purple.400}",
          500: "{purple.500}",
          600: "{purple.600}",
          700: "{purple.700}",
          800: "{purple.800}",
          900: "{purple.900}",
          950: "{purple.950}",
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

app.mount("#app");
