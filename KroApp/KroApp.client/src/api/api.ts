import axios, { type AxiosRequestConfig, type AxiosResponse } from "axios";

// Create an Axios instance with default settings
const apiClient = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || "https://localhost:7178/api",
  headers: {
    "Content-Type": "application/json",
    Accept: "application/json",
  },
});

const handleError = (error: any) => {
  console.error("API Error:", error);
  throw error;
};

apiClient.interceptors.request.use((config) => {
  const token = localStorage.getItem("authToken");
  if (token) {
    config.headers["Authorization"] = `Bearer ${token}`;
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
