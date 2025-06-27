import { View, Text, TouchableOpacity } from "react-native";
import { useAuth } from "@/contexts/AuthContext";
import { useRouter } from "expo-router";

export default function Home() {
  const { auth } = useAuth();
  const router = useRouter();

  return (
    <View
      style={{
        flex: 1,
        justifyContent: "center",
        alignItems: "center",
        backgroundColor: "#121212",
        padding: 20,
      }}
    >
      <Text
        style={{
          color: "#fff",
          fontSize: 24,
          fontWeight: "bold",
          marginBottom: 16,
        }}
      >
        Bem-vindo ao todoApp!
      </Text>

      {auth ? (
        <>
          <Text style={{ color: "#aaa", marginBottom: 24 }}>
            Logado como: {`${auth.username}`} ({`${auth.email}`})
          </Text>

          <TouchableOpacity
            onPress={() => router.push("/(tabs)/todotask")}
            style={{
              backgroundColor: "#4CAF50",
              paddingVertical: 12,
              paddingHorizontal: 32,
              borderRadius: 8,
            }}
          >
            <Text style={{ color: "#fff", fontSize: 16 }}>Ver Tarefas</Text>
          </TouchableOpacity>
        </>
      ) : (
        <Text style={{ color: "#aaa" }}>Usuário não autenticado</Text>
      )}
    </View>
  );
}
