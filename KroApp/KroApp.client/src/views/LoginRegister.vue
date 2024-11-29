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
                id="username-input"
                v-model="username"
                name="username"
                class="p-mb-3"
                fluid
                v-tooltip="'Your username can be seen by other users.'"
              />
              <label for="username-input">Username</label>
            </FloatLabel>
            <Message
              v-if="($form as any).username?.invalid"
              severity="error"
              size="small"
              variant="simple"
            >
              {{ ($form as any).username.error?.message }}
            </Message>
          </FormField>
          <FormField
            class="form-field"
            v-if="!isLogin"
          >
            <FloatLabel variant="on">
              <InputText
                id="email-input"
                v-model="email"
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
                v-model="password"
                name="password"
                class="p-mb-3"
                type="password"
                toggle-mask
                fluid
              >
                <template #footer>
                  <p class="mt-2">Requires at least:</p>
                  <ul class="pl-2 ml-2 mt-0">
                    <li>6 characters</li>
                    <li>1 lowercase letter</li>
                    <li>1 uppercase letter</li>
                    <li>1 number</li>
                    <li>1 special character</li>
                  </ul>
                </template>
              </Password>
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
          <FormField v-if="isLogin">
            <Checkbox
              id="remember-me"
              v-model="rememberMe"
              name="rememberMe"
              value="Remember Me"
            />
            <label for="ingredient2"> Remember Me? </label>
          </FormField>
          <FormField
            v-else
            class="form-field"
          >
            <FloatLabel variant="on">
              <Password
                v-if="!isLogin"
                id="confirm-password-input"
                v-model="confirmPassword"
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
import Checkbox from "primevue/checkbox";
import { zodResolver } from "@primevue/forms/resolvers/zod";
import { z } from "zod";
import auth from "@/api/auth";
import { useToast } from "primevue/usetoast";
import { useRouter } from "vue-router";
import { onMounted } from "vue";

const router = useRouter();
const toast = useToast();
const username = ref("");
const email = ref("");
const password = ref("");
const confirmPassword = ref("");
const rememberMe = ref(false);
const isLogin = ref(true);

const resolver = ref(
  zodResolver(
    z
      .object({
        username: isLogin.value
          ? z.string().optional()
          : z.string().min(1, { message: "Username is required." }),
        email: z
          .string()
          .min(1, { message: "Email is required." })
          .email({ message: "Invalid email address." }),
        password: z
          .string()
          .min(6, { message: "Password must be at least 6 characters long." })
          .regex(/[a-z]/, {
            message: "Password must contain at least one lowercase letter.",
          })
          .regex(/[A-Z]/, {
            message: "Password must contain at least one uppercase letter.",
          })
          .regex(/\d/, { message: "Password must contain at least one digit." })
          .regex(/[^a-zA-Z0-9]/, {
            message: "Password must contain at least one special character.",
          }),
        confirmPassword: z.string(),
      })
      .refine((data) => data.password === data.confirmPassword, {
        message: "Passwords don't match",
        path: ["confirmPassword"],
      }),
  ),
);

onMounted(() => {
  if (auth.isAuthenticated()) {
    router.push({ name: "User Home" });
  }
});

const onFormSubmit = async (form: any) => {
  let success = false;

  if (form.valid) {
    if (isLogin.value) {
      success = await handleLogin();
    } else {
      success = await handleRegister();
    }
  }

  if (success) {
    router.push({ name: "User Home" });
  }
};

function toggleAuthMode() {
  isLogin.value = !isLogin.value;
}

async function handleLogin() {
  const response = await auth.loginUser({
    username: username.value,
    password: password.value,
  });
  if (response?.token) {
    auth.saveToken(response.token);
    toast.add({
      severity: "success",
      summary: "Login successful",
      detail: "You are now logged in.",
      life: 3000,
    });
    return true;
  } else {
    toast.add({
      severity: "error",
      summary: "Login failed",
      detail: response.data[""].join("\n"),
      life: 3000,
    });
    return false;
  }
}

async function handleRegister() {
  const response = await auth.registerUser({
    username: username.value,
    email: email.value,
    password: password.value,
    confirmPassword: confirmPassword.value,
  });
  if (response?.token) {
    auth.saveToken(response.token);
    toast.add({
      severity: "success",
      summary: "Registration successful",
      detail: "Your account has been created.",
      life: 3000,
    });
    return true;
  } else {
    toast.add({
      severity: "error",
      summary: "Registration failed",
      detail: response.data[""].join("\n"),
      life: 3000,
    });
    return false;
  }
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

.p-tooltip {
  width: fit-content;
  font-size: 0.9rem;
  line-height: 1.4;
}
</style>
