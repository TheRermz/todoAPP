import React, { useEffect, useState } from "react";
import { FlatList, View, Text, ActivityIndicator, TouchableOpacity } from "react-native";
import { useRouter } from "expo-router";
import { fetchTasks } from "@/services/taskService";

interface Task {
  id: number;
  title: string;
  status: number;
  priority: number;
  startDate: string;
}

export default function TarefasScreen() {
  const [tasks, setTasks] = useState<Task[]>([]);
  const [loading, setLoading] = useState(true);
  const router = useRouter();

  useEffect(() => {
    async function loadTasks() {
      try {
        const data = await fetchTasks();
        setTasks(data);
      } catch (error) {
        console.error("Erro ao buscar tarefas:", error);
      } finally {
        setLoading(false);
      }
    }

    loadTasks();
  }, []);

  if (loading) {
    return <ActivityIndicator size="large" style={{ marginTop: 40 }} />;
  }

  return (
    <FlatList
      data={tasks}
      keyExtractor={(item) => item.id.toString()}
      contentContainerStyle={{ padding: 16 }}
      renderItem={({ item }) => (
        <TouchableOpacity
          onPress={() => router.push({
            pathname: "/todotask/[id]" as const,
            params: {id: String(item.id)}
          })}
          style={{
            padding: 16,
            marginBottom: 12,
            backgroundColor: "#1e1e1e",
            borderRadius: 10,
          }}
        >
          <Text style={{ fontSize: 18, fontWeight: "bold", color: "#fff" }}>
            {item.title}
          </Text>
          <Text style={{ color: "#aaa" }}>
            Status: {item.status} | Prioridade: {item.priority}
          </Text>
          <Text style={{ color: "#aaa" }}>Início: {new Date(item.startDate).toLocaleDateString()}</Text>
        </TouchableOpacity>
      )}
    />
  );
}
