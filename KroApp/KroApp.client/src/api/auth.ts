import { post } from "@/api/api";
import type { AxiosResponse } from "axios";
import { jwtDecode } from "jwt-decode";

interface AuthResponse {
  message: string;
  token: string;
  data: {};
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
  return exp && exp > currentTime;
}

function isAuthenticated() {
  const token = localStorage.getItem("authToken");
  if (!token) return false;

  try {
    return isTokenExpired(token);
  } catch (error) {
    console.error("Invalid token:", error);
    return false;
  }
}

const registerUser = async (userData: UserRegister): Promise<AuthResponse> => {
  try {
    const response: AxiosResponse = await post<AuthResponse>(
      "/Account/register",
      userData,
    );
    return response.data;
  } catch (error) {
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
  } catch (error) {
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
}

export default {
  registerUser,
  loginUser,
  logout,
  isTokenExpired,
  isAuthenticated,
  saveToken,
  getToken,
};
