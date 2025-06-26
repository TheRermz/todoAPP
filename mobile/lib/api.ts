import axios from "axios";
import Constants from "expo-constants";

const API_URL = Constants.expoConfig?.extra?.API_URL;

export const api = axios.create({
  baseURL: API_URL,
  headers: {
    "Content-Type": "application/json",
  },
});

// (Opcional) Interceptor para adicionar token JWT automaticamente
api.interceptors.request.use(
  async (config) => {
    // Aqui você pode buscar o token do AsyncStorage ou SecureStore
    // const token = await SecureStore.getItemAsync('token');
    const token = null; // substituir quando tiver auth

    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }

    return config;
  },
  (error) => Promise.reject(error)
);
