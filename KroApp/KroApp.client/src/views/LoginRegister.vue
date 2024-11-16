<template>
  <div class="login-card-container">
    <Card class="card">
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
          <FormField class="form-field">
            <FloatLabel variant="on">
              <InputText
                id="email-input"
                name="email"
                class="p-mb-3"
                fluid
              />
              <label for="email-input">Email</label>
            </FloatLabel>
            <Message
              v-if="($form as any).email?.invalid"
              severity="error"
              size="small"
              variant="simple"
            >
              {{ ($form as any).email.error?.message }}
            </Message>
          </FormField>

          <FormField class="form-field">
            <FloatLabel variant="on">
              <Password
                id="password-input"
                name="password"
                class="p-mb-3"
                type="password"
                toggle-mask
                fluid
              />
              <label for="password-input">Password</label>
            </FloatLabel>
            <Message
              v-if="($form as any).password?.invalid"
              severity="error"
              size="small"
              variant="simple"
            >
              {{ ($form as any).password.error?.message }}
            </Message>
          </FormField>
          <FormField
            v-if="!isLogin"
            class="form-field"
          >
            <FloatLabel variant="on">
              <Password
                v-if="!isLogin"
                id="confirm-password-input"
                name="confirmPassword"
                class="p-mb-3"
                type="password"
                toggle-mask
                fluid
                :feedback="false"
              />
              <label for="confirm-password-input">Confirm Password</label>
            </FloatLabel>
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
            variant="link"
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
  </div>
</template>

<script setup lang="ts">
import { ref } from "vue";
import {
  InputText,
  Password,
  Button,
  Card,
  Message,
  FloatLabel,
} from "primevue";
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
.login-card-container {
  max-width: 500px;
  margin: auto;
  padding: 2rem;
}

.form-field {
  margin-bottom: 0.5rem;
}
</style>
