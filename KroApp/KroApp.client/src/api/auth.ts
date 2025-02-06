import { post } from "@/api/api";
import router from "@/router";
import type { AxiosResponse } from "axios";
import { jwtDecode } from "jwt-decode";

interface AuthResponse {
  message: string;
  token: string;
  data: { "": [] };
}

interface JwtPayload {
  exp: number;
}

interface UserRegister {
  username: string;
  email: string;
  password: string;
  confirmPassword: string;
}

interface UserLogin {
  username: string;
  password: string;
}

function isTokenExpired(token: string): boolean {
  const { exp } = jwtDecode<JwtPayload>(token);
  const currentTime = Math.floor(Date.now() / 1000);
  return (!exp || exp < currentTime) as boolean;
}

function isAuthenticated() {
  const token = getToken();
  if (!token) return false;

  try {
    return !isTokenExpired(token);
  } catch (error) {
    console.error("Invalid token:", error);
    return false;
  }
}

function getUsername(token: string): string | null {
  try {
    const decoded = jwtDecode<JwtPayload & { given_name?: string }>(token);
    return decoded.given_name || null;
  } catch (error) {
    console.error("Invalid JWT:", error);
    return null;
  }
}

const registerUser = async (userData: UserRegister): Promise<AuthResponse> => {
  try {
    const response: AxiosResponse = await post<AuthResponse>(
      "/Account/register",
      userData,
    );
    return response.data;
  } catch (error: any) {
    return error.response;
  }
};

const loginUser = async (credentials: UserLogin): Promise<AuthResponse> => {
  try {
    const response: AxiosResponse = await post<AuthResponse>(
      "/Account/login",
      credentials,
    );
    return response.data;
  } catch (error: any) {
    return error.response;
  }
};

function saveToken(token: string) {
  localStorage.setItem("authToken", token);
}

function getToken(): string | null {
  return localStorage.getItem("authToken");
}

function removeToken() {
  localStorage.removeItem("authToken");
}

function logout() {
  removeToken();
  router.push({ name: "Authenticate" });
}

export default {
  registerUser,
  loginUser,
  logout,
  isTokenExpired,
  isAuthenticated,
  getUsername,
  saveToken,
  removeToken,
  getToken,
};
