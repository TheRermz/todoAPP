import { useLocalSearchParams, useRouter } from "expo-router";
import { View, Text, ActivityIndicator, ScrollView, TouchableOpacity } from "react-native";
import { useEffect, useState } from "react";
import { fetchTaskById } from "@/services/taskService";
import { TaskStatus, TaskPriority } from "@/types/tasks";

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

export default function TaskDetailScreen() {
  const { id } = useLocalSearchParams<{ id: string }>();
  const router = useRouter();
  const [task, setTask] = useState<Task | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (!id) return;

    const fetchTask = async () => {
      try {
        const data = await fetchTaskById(Number(id));
        setTask(data);
      } catch (error) {
        console.error(`Erro ao carregar tarefa:`, error);
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
    <ScrollView style={{ flex: 1, backgroundColor: "#121212" }} contentContainerStyle={{ padding: 16 }}>
      <Text style={{ fontSize: 24, fontWeight: "bold", color: "#fff" }}>{task.title}</Text>

      {task.description && (
        <Text style={{ fontSize: 16, color: "#ccc", marginTop: 8 }}>{task.description}</Text>
      )}

      <Text style={{ color: "#aaa", marginTop: 12 }}>
        {`Início: ${new Date(task.startDate).toLocaleString()}`}
      </Text>

      {task.endDate && (
        <Text style={{ color: "#aaa" }}>{`Prazo: ${new Date(task.endDate).toLocaleString()}`}</Text>
      )}

      <Text style={{ color: "#aaa" }}>
        {`Criado em: ${new Date(task.createdAt).toLocaleString()}`}
      </Text>

      {task.updatedAt && (
        <Text style={{ color: "#aaa" }}>
          {`Atualizado em: ${new Date(task.updatedAt).toLocaleString()}`}
        </Text>
      )}

      <Text style={{ color: "#aaa", marginTop: 8 }}>
        {`Status: ${statusLabels[task.status as TaskStatus] ?? "Desconhecido"}`}
      </Text>

      <Text style={{ color: "#aaa" }}>
        {`Prioridade: ${priorityLabels[task.priority as TaskPriority] ?? "Desconhecida"}`}
      </Text>

      {Array.isArray(task.tagList) && task.tagList.length > 0 && (
  <View style={{ marginTop: 12 }}>
    <Text style={{ fontWeight: "bold", color: "#fff" }}>Tags:</Text>
    {task.tagList.map((tag) => (
      <Text key={tag.id} style={{ color: "#ccc" }}>{`- ${tag.name}`}</Text>
    ))}
  </View>
)}



      <TouchableOpacity
        style={{
          marginTop: 24,
          backgroundColor: "#3b82f6",
          paddingVertical: 12,
          borderRadius: 8,
        }}
        onPress={() =>
          router.push({
            pathname: "/edit-task/[id]",
            params: { id: String(task.id) },
          })
        }
      >
        <Text style={{ color: "#fff", textAlign: "center", fontWeight: "bold" }}>
          Editar Tarefa
        </Text>
      </TouchableOpacity>
    </ScrollView>
  );
}
