// app/(tabs)/todotask/[id].tsx

import { useLocalSearchParams } from "expo-router";
import { View, Text, ActivityIndicator, ScrollView } from "react-native";
import { useEffect, useState } from "react";
import { fetchTaskById } from "@/services/taskService";

interface Task {
  id: number;
  title: string;
  description?: string;
  startDate: string;
  endDate?: string;
  status: number;
  priority: number;
  createdAt: string;
  updatedAt?: string;
  tagList?: { id: number; name: string }[];
}

export default function TaskDetailScreen() {
  const { id } = useLocalSearchParams<{ id: string }>();
  const [task, setTask] = useState<Task | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (!id) return;

    const fetchTask = async () => {
      try {
        const data = await fetchTaskById(Number(id));
        setTask(data);
      } catch (error) {
        console.error("Erro ao carregar tarefa:", error);
      } finally {
        setLoading(false);
      }
    };

    fetchTask();
  }, [id]);

  if (loading) {
    return (
      <View style={{ flex: 1, justifyContent: "center", alignItems: "center" }}>
        <ActivityIndicator size="large" />
      </View>
    );
  }

  if (!task) {
    return (
      <View style={{ padding: 16 }}>
        <Text style={{ color: "#fff" }}>Tarefa não encontrada.</Text>
      </View>
    );
  }

  return (
    <ScrollView contentContainerStyle={{ padding: 16 }}>
      <Text style={{ fontSize: 24, fontWeight: "bold", color: "#fff" }}>{task.title}</Text>
      {task.description ? (
        <Text style={{ fontSize: 16, color: "#ccc", marginTop: 8 }}>{task.description}</Text>
      ) : null}

      <Text style={{ color: "#aaa", marginTop: 12 }}>
        Início: {new Date(task.startDate).toLocaleString()}
      </Text>
      {task.endDate && (
        <Text style={{ color: "#aaa" }}>
          Prazo: {new Date(task.endDate).toLocaleString()}
        </Text>
      )}
      <Text style={{ color: "#aaa" }}>
        Criado em: {new Date(task.createdAt).toLocaleString()}
      </Text>
      {task.updatedAt && (
        <Text style={{ color: "#aaa" }}>
          Atualizado em: {new Date(task.updatedAt).toLocaleString()}
        </Text>
      )}

      <Text style={{ color: "#aaa", marginTop: 8 }}>
        Status: {["Pendente", "Em Andamento", "Concluída", "Cancelada"][task.status]}
      </Text>
      <Text style={{ color: "#aaa" }}>
        Prioridade: {["Baixa", "Média", "Alta"][task.priority]}
      </Text>

      {task.tagList?.length ? (
        <View style={{ marginTop: 12 }}>
          <Text style={{ fontWeight: "bold", color: "#fff" }}>Tags:</Text>
          {task.tagList.map((tag) => (
            <Text key={tag.id} style={{ color: "#ccc" }}>
              - {tag.name}
            </Text>
          ))}
        </View>
      ) : null}
    </ScrollView>
  );
}
