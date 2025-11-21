'use client'

import { useState } from 'react'
import { useRouter } from 'next/navigation'

export default function LoginPage() {
    const router = useRouter()
    const [isLogin, setIsLogin] = useState(true)
    const [formData, setFormData] = useState({
        email: '',
        password: '',
        firstName: '',
        lastName: '',
        documentType: 'CC',
        documentNumber: '',
        phone: '',
        city: ''
    })

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault()

        const endpoint = isLogin ? '/api/Auth/login' : '/api/Auth/register'
        const body = isLogin
            ? { email: formData.email, password: formData.password }
            : formData

        try {
            const res = await fetch(`http://localhost:5242${endpoint}`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(body)
            })

            if (res.ok) {
                const data = await res.json()
                localStorage.setItem('token', data.token)
                localStorage.setItem('user', JSON.stringify(data))
                alert(isLogin ? 'Sesión iniciada' : 'Registro exitoso')
                router.push('/')
            } else {
                const error = await res.json()
                alert(error.message || 'Error en la operación')
            }
        } catch (err) {
            alert('Error de conexión')
        }
    }

    return (
        <div className="container mx-auto px-4 py-12">
            <div className="max-w-md mx-auto bg-white rounded-lg shadow-lg p-8">
                <h1 className="text-3xl font-bold mb-6 text-center">
                    {isLogin ? 'Iniciar Sesión' : 'Registrarse'}
                </h1>

                <form onSubmit={handleSubmit} className="space-y-4">
                    {!isLogin && (
                        <>
                            <div className="grid grid-cols-2 gap-4">
                                <input
                                    type="text"
                                    placeholder="Nombre"
                                    className="border rounded px-3 py-2"
                                    value={formData.firstName}
                                    onChange={e => setFormData({ ...formData, firstName: e.target.value })}
                                    required
                                />
                                <input
                                    type="text"
                                    placeholder="Apellido"
                                    className="border rounded px-3 py-2"
                                    value={formData.lastName}
                                    onChange={e => setFormData({ ...formData, lastName: e.target.value })}
                                    required
                                />
                            </div>
                            <div className="grid grid-cols-2 gap-4">
                                <select
                                    className="border rounded px-3 py-2"
                                    value={formData.documentType}
                                    onChange={e => setFormData({ ...formData, documentType: e.target.value })}
                                >
                                    <option value="CC">CC</option>
                                    <option value="CE">CE</option>
                                    <option value="NIT">NIT</option>
                                </select>
                                <input
                                    type="text"
                                    placeholder="Número de documento"
                                    className="border rounded px-3 py-2"
                                    value={formData.documentNumber}
                                    onChange={e => setFormData({ ...formData, documentNumber: e.target.value })}
                                    required
                                />
                            </div>
                            <input
                                type="tel"
                                placeholder="Teléfono"
                                className="w-full border rounded px-3 py-2"
                                value={formData.phone}
                                onChange={e => setFormData({ ...formData, phone: e.target.value })}
                            />
                            <input
                                type="text"
                                placeholder="Ciudad"
                                className="w-full border rounded px-3 py-2"
                                value={formData.city}
                                onChange={e => setFormData({ ...formData, city: e.target.value })}
                            />
                        </>
                    )}

                    <input
                        type="email"
                        placeholder="Correo electrónico"
                        className="w-full border rounded px-3 py-2"
                        value={formData.email}
                        onChange={e => setFormData({ ...formData, email: e.target.value })}
                        required
                    />
                    <input
                        type="password"
                        placeholder="Contraseña"
                        className="w-full border rounded px-3 py-2"
                        value={formData.password}
                        onChange={e => setFormData({ ...formData, password: e.target.value })}
                        required
                    />

                    <button
                        type="submit"
                        className="w-full bg-primary text-white py-2 rounded-lg hover:bg-amber-600 transition"
                    >
                        {isLogin ? 'Ingresar' : 'Registrarse'}
                    </button>
                </form>

                <p className="text-center mt-4">
                    {isLogin ? '¿No tienes cuenta?' : '¿Ya tienes cuenta?'}
                    <button
                        onClick={() => setIsLogin(!isLogin)}
                        className="text-primary ml-2 hover:underline"
                    >
                        {isLogin ? 'Regístrate' : 'Inicia sesión'}
                    </button>
                </p>
            </div>
        </div>
    )
}
