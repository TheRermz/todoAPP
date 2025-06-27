import React, { useEffect, useState } from "react";
import { FlatList, View, Text, ActivityIndicator, TouchableOpacity } from "react-native";
import { useRouter } from "expo-router";
import { fetchTasks } from "@/services/taskService";
import {TaskPriority, TaskStatus} from "@/types/tasks"

interface Task {
  id: number;
  title: string;
  status: number;
  priority: number;
  startDate: string;
}

const statusLabels: Record<TaskStatus, string> = {
  [TaskStatus.Pending]: "Pendente",
  [TaskStatus.InProgress]: "Em andamento",
  [TaskStatus.Completed]: "Concluída",
  [TaskStatus.Late]: "Atrasada",
};

const priorityLabels: Record<TaskPriority, string> = {
  [TaskPriority.Low]: "Baixa",
  [TaskPriority.Medium]: "Média",
  [TaskPriority.High]: "Alta",
};

const getStatusLabel = (status: number): string => {
  return statusLabels[status as TaskStatus] ?? "Desconhecido";
};

const getPriorityLabel = (priority: number): string => {
  return priorityLabels[priority as TaskPriority] ?? "Desconhecida";
};

export default function TarefasScreen() {
  const [tasks, setTasks] = useState<Task[]>([]);
  const [loading, setLoading] = useState(true);
  const router = useRouter();

  useEffect(() => {
    const loadTasks = async () => {
      try {
        const data = await fetchTasks();
        setTasks(data);
      } catch (error) {
        console.error(`Erro ao buscar tarefas: ${error}`);
      } finally {
        setLoading(false);
      }
    };

    loadTasks();
  }, []);

  if (loading) {
    return (
      <View style={{ flex: 1, justifyContent: "center", alignItems: "center", backgroundColor: "#121212" }}>
        <ActivityIndicator size="large" color="#00ff99" />
      </View>
    );
  }

  return (
    <View style={{ flex: 1, backgroundColor: "#121212", padding: 16 }}>
      <Text style={{ fontSize: 24, fontWeight: "bold", color: "#fff", marginBottom: 16 }}>
        Suas Tarefas
      </Text>

      <FlatList
        data={tasks}
        keyExtractor={(item) => `${item.id}`}
        renderItem={({ item }) => (
          <TouchableOpacity
            onPress={() =>
              router.push({
                pathname: `/todotask/[id]` as const,
                params: { id: `${item.id}` },
              })
            }
            style={{
              padding: 16,
              marginBottom: 12,
              backgroundColor: "#1e1e1e",
              borderRadius: 10,
              borderWidth: 1,
              borderColor: "#333",
            }}
          >
            <Text style={{ fontSize: 18, fontWeight: "bold", color: "#fff" }}>
              {`${item.title}`}
            </Text>
            <Text style={{ color: "#bbb", marginTop: 4 }}>
              Status: {`${getStatusLabel(item.status)}`} | Prioridade: {`${getPriorityLabel(item.priority)}`}
            </Text>
            <Text style={{ color: "#bbb", marginTop: 2 }}>
              Início: {new Date(item.startDate).toLocaleDateString()}
            </Text>
          </TouchableOpacity>
        )}
      />
    </View>
  );
}
