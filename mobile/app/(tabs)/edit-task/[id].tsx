import { useLocalSearchParams, useRouter } from "expo-router";
import {
  View,
  Text,
  TextInput,
  ScrollView,
  Button,
  ActivityIndicator,
  Alert,
  StyleSheet,
  TouchableOpacity,
  Platform,
} from "react-native";
import { useEffect, useState } from "react";
import DateTimePicker from "@react-native-community/datetimepicker";
import { fetchTaskById, updateTask } from "@/services/taskService";
import { TaskStatus, TaskPriority } from "@/types/tasks";

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

export default function EditTaskScreen() {
  const { id } = useLocalSearchParams<{ id: string }>();
  const router = useRouter();

  const [form, setForm] = useState({
    title: "",
    description: "",
    startDate: new Date(),
    endDate: new Date(),
    status: TaskStatus.Pending,
    priority: TaskPriority.Low,
  });

  const [loading, setLoading] = useState(true);
  const [showStartPicker, setShowStartPicker] = useState(false);
  const [showEndPicker, setShowEndPicker] = useState(false);

  useEffect(() => {
    if (!id) return;

    const loadTask = async () => {
      try {
        const data = await fetchTaskById(Number(id));
        setForm({
          title: data.title,
          description: data.description || "",
          startDate: new Date(data.startDate),
          endDate: data.endDate ? new Date(data.endDate) : new Date(),
          status: data.status,
          priority: data.priority,
        });
      } catch (err) {
        Alert.alert("Erro", "Não foi possível carregar a tarefa.");
      } finally {
        setLoading(false);
      }
    };

    loadTask();
  }, [id]);

  const handleUpdate = async () => {
    try {
      await updateTask(Number(id), {
        ...form,
        startDate: form.startDate.toISOString(),
        endDate: form.endDate.toISOString(),
      });
      Alert.alert("Sucesso", "Tarefa atualizada!");
      router.back();
    } catch (err) {
      Alert.alert("Erro", "Não foi possível atualizar a tarefa.");
    }
  };

  const formatDateTime = (date: Date) => {
    return `${date.toLocaleDateString()} ${date.toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" })}`;
  };

  if (loading) {
    return (
      <View style={styles.centered}>
        <ActivityIndicator size="large" />
      </View>
    );
  }

  return (
    <ScrollView contentContainerStyle={styles.container}>
      <Text style={styles.title}>Editar Tarefa</Text>

      <TextInput
        style={styles.input}
        placeholder="Título"
        placeholderTextColor="#888"
        value={form.title}
        onChangeText={(text) => setForm({ ...form, title: text })}
      />

      <TextInput
        style={[styles.input, { height: 100 }]}
        placeholder="Descrição"
        placeholderTextColor="#888"
        multiline
        value={form.description}
        onChangeText={(text) => setForm({ ...form, description: text })}
      />

      <Text style={styles.label}>Início:</Text>
      <TouchableOpacity onPress={() => setShowStartPicker(true)} style={styles.dateButton}>
        <Text style={styles.optionText}>{formatDateTime(form.startDate)}</Text>
      </TouchableOpacity>
      {showStartPicker && (
        <DateTimePicker
          value={form.startDate}
          mode="datetime"
          display={Platform.OS === "ios" ? "spinner" : "default"}
          onChange={(_, date) => {
            setShowStartPicker(false);
            if (date) setForm({ ...form, startDate: date });
          }}
        />
      )}

      <Text style={styles.label}>Prazo Final:</Text>
      <TouchableOpacity onPress={() => setShowEndPicker(true)} style={styles.dateButton}>
        <Text style={styles.optionText}>{formatDateTime(form.endDate)}</Text>
      </TouchableOpacity>
      {showEndPicker && (
        <DateTimePicker
          value={form.endDate}
          mode="datetime"
          display={Platform.OS === "ios" ? "spinner" : "default"}
          onChange={(_, date) => {
            setShowEndPicker(false);
            if (date) setForm({ ...form, endDate: date });
          }}
        />
      )}

      <Text style={styles.label}>Status</Text>
      {Object.values(TaskStatus)
        .filter((v) => typeof v === "number")
        .map((status) => (
          <TouchableOpacity
            key={status}
            style={[
              styles.optionButton,
              form.status === Number(status) && styles.optionSelected,
            ]}
            onPress={() => setForm({ ...form, status: Number(status) })}
          >
            <Text style={styles.optionText}>{statusLabels[status as TaskStatus]}</Text>
          </TouchableOpacity>
        ))}

      <Text style={styles.label}>Prioridade</Text>
      {Object.values(TaskPriority)
        .filter((v) => typeof v === "number")
        .map((priority) => (
          <TouchableOpacity
            key={priority}
            style={[
              styles.optionButton,
              form.priority === Number(priority) && styles.optionSelected,
            ]}
            onPress={() => setForm({ ...form, priority: Number(priority) })}
          >
            <Text style={styles.optionText}>{priorityLabels[priority as TaskPriority]}</Text>
          </TouchableOpacity>
        ))}

      <TouchableOpacity style={styles.saveButton} onPress={handleUpdate}>
        <Text style={styles.saveButtonText}>Salvar alterações</Text>
      </TouchableOpacity>
    </ScrollView>
  );
}

const styles = StyleSheet.create({
  container: {
    backgroundColor: "#121212",
    padding: 16,
    flexGrow: 1,
  },
  centered: {
    flex: 1,
    backgroundColor: "#121212",
    justifyContent: "center",
    alignItems: "center",
  },
  title: {
    fontSize: 24,
    color: "#fff",
    marginBottom: 16,
    fontWeight: "bold",
    textAlign: "center",
  },
  input: {
    backgroundColor: "#1e1e1e",
    color: "#fff",
    padding: 12,
    borderRadius: 8,
    marginBottom: 16,
  },
  label: {
    color: "#fff",
    fontSize: 16,
    marginBottom: 8,
    marginTop: 16,
    fontWeight: "bold",
  },
  optionButton: {
    padding: 10,
    backgroundColor: "#1e1e1e",
    borderRadius: 6,
    marginBottom: 8,
  },
  optionSelected: {
    backgroundColor: "#3b82f6",
  },
  optionText: {
    color: "#fff",
    textAlign: "center",
  },
  dateButton: {
    backgroundColor: "#1e1e1e",
    padding: 12,
    borderRadius: 8,
    marginBottom: 8,
  },
  saveButton: {
    marginTop: 24,
    backgroundColor: "#22c55e",
    paddingVertical: 14,
    borderRadius: 10,
  },
  saveButtonText: {
    color: "#fff",
    fontWeight: "bold",
    fontSize: 16,
    textAlign: "center",
  },
});
