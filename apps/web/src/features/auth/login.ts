import { ApiError, http } from '../../lib/http.ts'

export interface LoginResult {
  token: string
  expiresAt: string
  user: {
    navigationId: string
    nome: string
    email: string
  }
}

export async function login(email: string, password: string): Promise<LoginResult> {
  try {
    return await http<LoginResult>('/api/Auth/login', {
      method: 'POST',
      body: { email, senha: password },
    })
  } catch (error) {
    if (error instanceof ApiError && error.status === 401) {
      throw new Error('E-mail ou senha inválidos.')
    }
    throw error
  }
}
