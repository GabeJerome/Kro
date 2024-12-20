import axios, { type AxiosRequestConfig, type AxiosResponse } from "axios";
import auth from "@/api/auth";

// Create an Axios instance with default settings
const apiClient = axios.create({
  baseURL: import.meta.env.VITE_API_URL,
  headers: {
    "Content-Type": "application/json",
    Accept: "application/json",
  },
});

const handleError = (error: any) => {
  throw error;
};

apiClient.interceptors.request.use((config) => {
  const token = auth.getToken();
  if (token && !auth.isTokenExpired(token)) {
    config.headers["Authorization"] = `Bearer ${token}`;
  } else {
    auth.logout();
  }
  return config;
});

// GET request
export const get = async <T>(
  url: string,
  config?: AxiosRequestConfig,
): Promise<AxiosResponse<T>> => {
  try {
    const response: AxiosResponse = await apiClient.get<T>(url, config);
    return response;
  } catch (error) {
    return handleError(error);
  }
};

// POST request
export const post = async <T>(
  url: string,
  data: any,
  config?: AxiosRequestConfig,
): Promise<AxiosResponse<T>> => {
  try {
    const response: AxiosResponse = await apiClient.post<T>(url, data, config);
    return response;
  } catch (error) {
    return handleError(error);
  }
};

// PUT request
export const put = async <T>(
  url: string,
  data: any,
  config?: AxiosRequestConfig,
): Promise<AxiosResponse<T>> => {
  try {
    const response: AxiosResponse = await apiClient.put<T>(url, data, config);
    return response;
  } catch (error) {
    return handleError(error);
  }
};

// DELETE request
export const del = async <T>(
  url: string,
  config?: AxiosRequestConfig,
): Promise<AxiosResponse<T>> => {
  try {
    const response: AxiosResponse = await apiClient.delete<T>(url, config);
    return response;
  } catch (error) {
    return handleError(error);
  }
};
