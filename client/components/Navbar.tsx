'use client'

import { useEffect, useState } from 'react'
import Link from 'next/link'
import { useRouter } from 'next/navigation'

export default function Navbar() {
    const router = useRouter()
    const [user, setUser] = useState<any>(null)
    const [mounted, setMounted] = useState(false)

    useEffect(() => {
        setMounted(true)
        const checkUser = () => {
            const savedUser = localStorage.getItem('user')
            if (savedUser) {
                setUser(JSON.parse(savedUser))
            } else {
                setUser(null)
            }
        }

        checkUser()

        // Listen for storage events to update state across tabs/components
        window.addEventListener('storage', checkUser)

        // Custom event for login/logout updates within the same tab
        window.addEventListener('auth-change', checkUser)

        return () => {
            window.removeEventListener('storage', checkUser)
            window.removeEventListener('auth-change', checkUser)
        }
    }, [])

    const logout = () => {
        localStorage.removeItem('token')
        localStorage.removeItem('user')
        localStorage.removeItem('cart')
        setUser(null)
        window.dispatchEvent(new Event('auth-change'))
        router.push('/')
    }

    // Prevent hydration mismatch
    if (!mounted) return null

    return (
        <nav className="bg-secondary text-white shadow-lg">
            <div className="container mx-auto px-4 py-4">
                <div className="flex justify-between items-center">
                    <Link href="/" className="text-2xl font-bold text-primary">
                        🏗️ FIRMEZA
                    </Link>
                    <div className="flex gap-6 items-center">
                        <Link href="/" className="hover:text-primary transition">
                            Productos
                        </Link>
                        <Link href="/cart" className="hover:text-primary transition">
                            Carrito
                        </Link>

                        {user ? (
                            <div className="flex items-center gap-4">
                                <span className="text-sm text-gray-300">
                                    Hola, {user.firstName}
                                </span>
                                <button
                                    onClick={logout}
                                    className="bg-red-600 hover:bg-red-700 text-white px-4 py-1 rounded text-sm transition"
                                >
                                    Salir
                                </button>
                            </div>
                        ) : (
                            <Link
                                href="/login"
                                className="bg-primary hover:bg-amber-600 text-white px-4 py-2 rounded transition"
                            >
                                Ingresar
                            </Link>
                        )}
                    </div>
                </div>
            </div>
        </nav>
    )
}
