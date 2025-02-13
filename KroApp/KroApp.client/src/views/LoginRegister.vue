<template>
  <div class="login-card-container">
    <Card class="card">
      <template #title>
        <h2>{{ isLogin ? "Login" : "Register" }}</h2>
      </template>
      <template #content>
        <Form
          :resolver="resolver"
          class="flex justify-center flex-col gap-4"
          @submit="onFormSubmit"
        >
          <FormField class="form-field">
            <FloatLabel variant="on">
              <InputText
                id="username-input"
                v-model="username"
                v-tooltip="'Your username can be seen by other users.'"
                name="username"
                class="p-mb-3"
                fluid
              />
              <label for="username-input"> Username </label>
            </FloatLabel>
            <Message
              v-if="formErrors.username"
              severity="error"
              size="small"
              variant="simple"
            >
              {{ formErrors.username[0] }}
            </Message>
          </FormField>
          <FormField
            v-if="!isLogin"
            class="form-field"
          >
            <FloatLabel variant="on">
              <InputText
                id="email-input"
                v-model="email"
                name="email"
                class="p-mb-3"
                fluid
              />
              <label for="email-input"> Email </label>
            </FloatLabel>
            <Message
              v-if="formErrors.email"
              severity="error"
              size="small"
              variant="simple"
            >
              {{ formErrors.email[0] }}
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
                :feedback="true"
                feedback-tooltip
                prompt-label="Enter a password"
              >
                <template #footer>
                  <Message
                    v-if="formErrors.password"
                    severity="error"
                    size="small"
                    variant="simple"
                  >
                    <p class="mt-2">Still required:</p>
                    <ul>
                      <li
                        v-for="error in formErrors.password"
                        :key="error"
                      >
                        {{ error }}
                      </li>
                    </ul>
                  </Message>
                </template>
              </Password>
              <label for="password-input"> Password </label>
              <Message
                v-if="formErrors.password && submitAttempted"
                severity="error"
                size="small"
                variant="simple"
              >
                Invalid Password
              </Message>
            </FloatLabel>
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
              <label for="confirm-password-input"> Confirm Password </label>
            </FloatLabel>
            <Message
              v-if="formErrors.confirmPassword"
              severity="error"
              size="small"
              variant="simple"
            >
              {{ formErrors.confirmPassword[0] }}
            </Message>
          </FormField>

          <label for="toggle-login-register">{{
            isLogin ? "Don't have an account?" : "Already have an account?"
          }}</label>
          <Button
            :label="`${isLogin ? 'Register' : 'Login'}`"
            name="toggle-login-register"
            variant="text"
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
import { z, ZodError } from "zod";
import auth from "@/api/auth";
import { useToast } from "primevue/usetoast";
import { useRouter } from "vue-router";
import { onMounted } from "vue";

const router = useRouter();
const toast = useToast();
const username = ref<string>("");
const email = ref<string>("");
const password = ref<string>("");
const confirmPassword = ref<string>("");
const rememberMe = ref<boolean>(false);
const isLogin = ref(true);
const formErrors = ref<Record<string, string[]>>({});
const submitAttempted = ref<boolean>(false);

const schema = z
  .object({
    username: z.string().min(1, { message: "Username is required." }),
    email: z
      .string()
      .min(1, { message: "Email is required." })
      .email({ message: "Invalid email address." })
      .optional()
      .refine((value) => isLogin.value || value, {
        message: "Email is required.",
      }),
    password: z
      .string()
      .min(6, { message: "Must be at least 6 characters long." })
      .regex(/[a-z]/, {
        message: "Must contain at least one lowercase letter.",
      })
      .regex(/[A-Z]/, {
        message: "Must contain at least one uppercase letter.",
      })
      .regex(/\d/, { message: "Must contain at least one digit." })
      .regex(/[^a-zA-Z0-9]/, {
        message: "Must contain at least one special character.",
      }),
    confirmPassword: z
      .string()
      .optional()
      .refine((value) => isLogin.value || value, {
        message: "Please confirm your password.",
      }),
  })
  .refine((data) => isLogin.value || data.password === data.confirmPassword, {
    message: "Passwords don't match",
    path: ["confirmPassword"],
  });

const resolver = (data: Record<string, any>) => {
  try {
    const formData = {
      username: data.values.username || "",
      email: isLogin.value ? undefined : data.values.email || "",
      password: data.values.password || "",
      confirmPassword: isLogin.value
        ? undefined
        : data.values.confirmPassword || "",
    };

    schema.parse(formData);
    formErrors.value = {};
    return { values: data, errors: {} };
  } catch (error) {
    if (error instanceof ZodError) {
      formErrors.value = error.errors.reduce(
        (accumulator, current) => {
          if (!accumulator[current.path[0]]) {
            accumulator[current.path[0]] = [];
          }
          accumulator[current.path[0]].push(current.message);
          return accumulator;
        },
        {} as Record<string, string[]>,
      );
    }
    return { values: {}, errors: formErrors.value };
  }
};

onMounted(() => {
  if (auth.isAuthenticated()) {
    router.push({ name: "User Home" });
  }
});

const onFormSubmit = async () => {
  submitAttempted.value = true;
  const formData = {
    username: username.value,
    email: isLogin.value ? undefined : email.value,
    password: password.value,
    confirmPassword: isLogin.value ? undefined : confirmPassword.value,
  };

  const { errors } = resolver({ values: formData });

  if (Object.keys(errors).length === 0) {
    let success = false;

    if (isLogin.value) {
      success = await handleLogin();
    } else {
      success = await handleRegister();
    }

    if (success) {
      router.push({ name: "User Home" });
    }
  } else {
    console.log("Form validation errors:", errors);
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
