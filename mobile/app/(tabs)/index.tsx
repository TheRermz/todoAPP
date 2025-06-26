import { View, Text } from "react-native";
import { useAuth } from "@/contexts/AuthContext";

export default function Home() {
  const { auth } = useAuth();

  return (
    <View style={{ flex: 1, justifyContent: "center", alignItems: "center", backgroundColor: "#121212" }}>
      <Text style={{ color: "#fff", fontSize: 20, marginBottom: 10 }}>
        Bem-vindo ao todoApp!
      </Text>
      {auth ? (
        <Text style={{ color: "#aaa" }}>
          Logado como: {auth.username} ({auth.email})
        </Text>
      ) : (
        <Text style={{ color: "#aaa" }}>Usuário não autenticado</Text>
      )}
    </View>
  );
}
