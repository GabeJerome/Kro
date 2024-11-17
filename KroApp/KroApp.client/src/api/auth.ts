import { post } from "@/api/api";
import type { AxiosResponse } from "axios";
import { jwtDecode } from "jwt-decode";

interface AuthResponse {
  message: string;
  token: string;
}

interface JwtPayload {
  exp: number;
}

function isTokenExpired(token: string): boolean {
  const { exp } = jwtDecode<JwtPayload>(token);
  const currentTime = Math.floor(Date.now() / 1000);
  return exp < currentTime;
}

const registerUser = async (userData: {
  email: string;
  password: string;
  confirmPassword: string;
}): Promise<AuthResponse | undefined> => {
  console.log("Registration Data:", userData);
  try {
    const response: AxiosResponse = await post<AuthResponse>(
      "/Account/register",
      userData,
    );

    if (response.status == 200) {
      return response.data;
    } else {
      return undefined;
    }
  } catch (error) {
    console.log("Registration error:", error);
    return undefined;
  }
};

const loginUser = async (credentials: {
  email: string;
  password: string;
}): Promise<AuthResponse | undefined> => {
  try {
    const response: AxiosResponse = await post<AuthResponse>(
      "/Account/login",
      credentials,
    );

    if (response.status === 200) {
      return response.data;
    } else if (response.status === 423) {
      return undefined;
    } else {
      return undefined;
    }
  } catch (error) {
    console.error("Login error:", error);
    return undefined;
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
  saveToken,
  getToken,
};
