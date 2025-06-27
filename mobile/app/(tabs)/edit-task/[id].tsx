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
} from "react-native";
import { useEffect, useState } from "react";
import { fetchTaskById, updateTask } from "@/services/taskService";
import { TaskStatus, TaskPriority } from "@/types/tasks";

export default function EditTaskScreen() {
  const { id } = useLocalSearchParams<{ id: string }>();
  const router = useRouter();

  const [form, setForm] = useState({
    title: "",
    description: "",
    status: TaskStatus.Pending,
    priority: TaskPriority.Medium,
  });

  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (!id) return;

    const loadTask = async () => {
      try {
        const data = await fetchTaskById(Number(id));
        setForm({
          title: data.title,
          description: data.description || "",
          status: data.status,
          priority: data.priority,
        });
      } catch (err) {
        Alert.alert(`Erro - ${err}`, "Não foi possível carregar a tarefa.");
      } finally {
        setLoading(false);
      }
    };

    loadTask();
  }, [id]);

  const handleUpdate = async () => {
    try {
      await updateTask(Number(id), form);
      Alert.alert("Sucesso", "Tarefa atualizada!");
      router.back();
    } catch (err) {
      Alert.alert(`Erro - ${err}`, "Não foi possível atualizar a tarefa.");
    }
  };

  if (loading) {
    return <ActivityIndicator style={{ marginTop: 40 }} size="large" />;
  }

  return (
    <ScrollView contentContainerStyle={{ padding: 16, backgroundColor: "#121212", flexGrow: 1 }}>
      <Text style={{ fontSize: 24, color: "#fff", marginBottom: 16 }}>Editar Tarefa</Text>

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

      <Text style={styles.label}>Status:</Text>
      {Object.values(TaskStatus)
        .filter((v) => typeof v === "number")
        .map((status) => (
          <Button
            key={status}
            title={TaskStatus[status as TaskStatus]}
            onPress={() => setForm({ ...form, status: Number(status) })}
            color={form.status === Number(status) ? "#3b82f6" : "#444"}
          />
        ))}

      <Text style={styles.label}>Prioridade:</Text>
      {Object.values(TaskPriority)
        .filter((v) => typeof v === "number")
        .map((priority) => (
          <Button
            key={priority}
            title={TaskPriority[priority as TaskPriority]}
            onPress={() => setForm({ ...form, priority: Number(priority) })}
            color={form.priority === Number(priority) ? "#3b82f6" : "#444"}
          />
        ))}

      <View style={{ marginTop: 24 }}>
        <Button title="Salvar alterações" onPress={handleUpdate} />
      </View>
    </ScrollView>
  );
}

const styles = StyleSheet.create({
  input: {
    backgroundColor: "#1e1e1e",
    color: "#fff",
    padding: 12,
    marginBottom: 16,
    borderRadius: 8,
  },
  label: {
    color: "#fff",
    marginTop: 16,
    marginBottom: 8,
    fontWeight: "bold",
  },
});
