<template>
  <Card class="login-card">
    <template #title>
      <h2>{{ isLogin ? "Login" : "Register" }}</h2>
    </template>

    <template #content>
      <Form
        v-slot="$form"
        :resolver
        class="flex justify-center flex-col gap-4"
        @submit="onFormSubmit"
      >
        <FormField class="flex flex-col gap-1">
          <InputText
            name="email"
            placeholder="Email"
            class="p-mb-3"
            fluid
          />
          <Message
            v-if="($form as any).email?.invalid"
            severity="error"
            size="small"
            variant="simple"
          >
            {{ ($form as any).email.error?.message }}
          </Message>
        </FormField>

        <FormField class="flex flex-col gap-1">
          <Password
            name="password"
            placeholder="Password"
            class="p-mb-3"
            type="password"
            toggle-mask
            fluid
          />
          <Message
            v-if="($form as any).password?.invalid"
            severity="error"
            size="small"
            variant="simple"
          >
            {{ ($form as any).password.error?.message }}
          </Message>
        </FormField>

        <FormField v-if="!isLogin">
          <Password
            v-if="!isLogin"
            name="confirmPassword"
            placeholder="Confirm Password"
            class="p-mb-3"
            type="password"
            toggle-mask
            fluid
          />
          <Message
            v-if="($form as any).confirmPassword?.invalid"
            severity="error"
            size="small"
            variant="simple"
          >
            {{ ($form as any).confirmPassword.error?.message }}
          </Message>
        </FormField>

        <label for="toggle-login-register">{{
          isLogin ? "Don't have an account?" : "Already have an account?"
        }}</label>
        <Button
          :label="`${isLogin ? 'Register' : 'Login'}`"
          name="toggle-login-register"
          class="p-mt-2 p-button-text"
          @click="toggleAuthMode"
        />

        <Button
          type="submit"
          severity="secondary"
          label="Submit"
        />
      </Form>
    </template>
  </Card>
</template>

<script setup lang="ts">
import { ref } from "vue";
import { InputText, Password, Button, Card, Message } from "primevue";
import { Form, FormField } from "@primevue/forms";
import { zodResolver } from "@primevue/forms/resolvers/zod";
import { z } from "zod";

const resolver = ref(
  zodResolver(
    z
      .object({
        email: z
          .string()
          .min(1, { message: "Email is required." })
          .email({ message: "Invalid email address." }),
        password: z.string().min(6, { message: "Password is required." }),
        confirmPassword: z.string(),
      })
      .refine((data) => data.password === data.confirmPassword, {
        message: "Passwords don't match",
        path: ["confirmPassword"],
      }),
  ),
);

const onFormSubmit = (form: any) => {
  if (form.valid) {
    if (isLogin.value) {
      console.log("Data is valid! Logging in...");
    } else {
      console.log("Data is valid! Registering...");
    }
  } else {
    console.log("Data is not valid");
  }
};

const isLogin = ref(true);

function toggleAuthMode() {
  isLogin.value = !isLogin.value;
}
</script>

<style scoped>
.login-card {
  max-width: 400px;
  margin: auto;
  padding: 2rem;
}
</style>
