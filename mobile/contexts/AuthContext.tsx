import React, { createContext, useContext, useEffect, useState } from 'react';
import * as SecureStore from 'expo-secure-store';
import {api} from '@/lib/api';
import { AuthData } from '@/types/auth';

interface AuthContextType {
  auth: AuthData | null;
  login: (email: string, password: string) => Promise<void>;
  logout: () => void;
  register: (email: string, username: string, password: string) => Promise<void>;
  loading: boolean;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [auth, setAuth] = useState<AuthData | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const loadStorage = async () => {
      const json = await SecureStore.getItemAsync('auth');
      if (json) setAuth(JSON.parse(json));
      setLoading(false);
    };
    loadStorage();
  }, []);

  const login = async (email: string, password: string) => {
  const res = await api.post('/Auth/login', { email, password });

  const raw = res.data;
  const data: AuthData = {
    token: raw.jwtToken, // <- renomeando corretamente
    username: raw.username,
    // adicione email ou userId aqui se necessário
  };

  setAuth(data);
  await SecureStore.setItemAsync('auth', JSON.stringify(data));
};

  const register = async (email: string, username: string, password: string) => {
    await api.post('/User/', { email, username, password });
    await login(email, password); // login automático após cadastro
  };

  const logout = async () => {
    setAuth(null);
    await SecureStore.deleteItemAsync('auth');
  };

  return (
    <AuthContext.Provider value={{ auth, login, logout, register, loading }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error('useAuth precisa estar dentro de AuthProvider');
  return ctx;
};
