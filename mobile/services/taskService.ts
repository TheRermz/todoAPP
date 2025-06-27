import { api } from "@/lib/api";
import * as SecureStore from "expo-secure-store";

export interface CreateTaskPayload {
  title: string;
  description?: string;
  startDate: string; // ISO format
  endDate?: string;
  status?: number;
  priority?: number;
  tagIds?: number[];
}

export async function fetchTasks() {
  const json = await SecureStore.getItemAsync("auth");
  if (!json) throw new Error("Usuário não autenticado");

  const auth = JSON.parse(json);
  const token = auth?.token;

  const res = await api.get("/TodoTask", {
    headers: {
      Authorization: `Bearer ${token}`,
    },
  });

  return res.data;
}

export async function fetchTaskById(id: number) {
  const json = await SecureStore.getItemAsync("auth");
  if (!json) throw new Error("Usuário não autenticado");

  const auth = JSON.parse(json);
  const token = auth?.token;
  const response = await api.get(`/todotask/${id}`, {
    headers: {
      Authorization: `Bearer ${token}`,
    },
  });
  return response.data;
}

export async function createTask(payload: CreateTaskPayload) {
  const response = await api.post("/todotask", payload);
  return response.data;
}

export async function updateTask(
  id: number,
  payload: Partial<CreateTaskPayload>
) {
  const response = await api.put(`/todotask/${id}`, payload);
  return response.data;
}

export async function deleteTask(id: number) {
  await api.delete(`/todotask/${id}`);
}
