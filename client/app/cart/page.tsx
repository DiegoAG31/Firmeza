'use client'

import { useEffect, useState } from 'react'
import { useRouter } from 'next/navigation'

interface CartItem {
    id: number
    name: string
    price: number
    quantity: number
    stock: number
}

export default function CartPage() {
    const router = useRouter()
    const [cart, setCart] = useState<CartItem[]>([])
    const [user, setUser] = useState<any>(null)

    useEffect(() => {
        const savedCart = JSON.parse(localStorage.getItem('cart') || '[]')
        setCart(savedCart)

        const savedUser = localStorage.getItem('user')
        if (savedUser) {
            setUser(JSON.parse(savedUser))
        }
    }, [])

    const updateQuantity = (id: number, delta: number) => {
        const updated = cart.map(item => {
            if (item.id === id) {
                const newQty = Math.max(1, Math.min(item.stock, item.quantity + delta))
                return { ...item, quantity: newQty }
            }
            return item
        })
        setCart(updated)
        localStorage.setItem('cart', JSON.stringify(updated))
    }

    const removeItem = (id: number) => {
        const updated = cart.filter(item => item.id !== id)
        setCart(updated)
        localStorage.setItem('cart', JSON.stringify(updated))
    }

    const checkout = async () => {
        if (!user) {
            alert('Debes iniciar sesión para realizar la compra')
            router.push('/login')
            return
        }

        const token = localStorage.getItem('token')
        const items = cart.map(item => ({
            productId: item.id,
            quantity: item.quantity
        }))

        console.log('Enviando compra:', {
            customerId: user.customerId,
            details: items
        })

        try {
            const apiUrl = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5242'
            const res = await fetch(`${apiUrl}/api/Sales`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                },
                body: JSON.stringify({
                    customerId: user.customerId,
                    details: items // Backend expects "Details", not "items"
                })
            })

            if (res.ok) {
                const data = await res.json()
                alert(`✅ ¡Compra exitosa!\n\nNúmero de venta: ${data.saleNumber}\n\n📧 Se ha enviado un comprobante a tu correo electrónico con el detalle de tu compra.`)
                localStorage.removeItem('cart')
                setCart([])
                router.push('/')
            } else {
                const error = await res.json()
                alert(error.message || 'Error al procesar la compra')
            }
        } catch (err) {
            alert('Error de conexión')
        }
    }

    const subtotal = cart.reduce((sum, item) => sum + (item.price * item.quantity), 0)
    const tax = subtotal * 0.19
    const total = subtotal + tax

    if (cart.length === 0) {
        return (
            <div className="container mx-auto px-4 py-12 text-center">
                <h1 className="text-3xl font-bold mb-4">Carrito Vacío</h1>
                <p className="text-gray-600 mb-6">No tienes productos en tu carrito</p>
                <a href="/" className="bg-primary text-white px-6 py-2 rounded-lg hover:bg-amber-600 transition">
                    Ver Productos
                </a>
            </div>
        )
    }

    return (
        <div className="container mx-auto px-4 py-12">
            <h1 className="text-3xl font-bold mb-8">Carrito de Compras</h1>

            <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
                <div className="lg:col-span-2 space-y-4">
                    {cart.map(item => (
                        <div key={item.id} className="bg-white rounded-lg shadow p-4 flex items-center gap-4">
                            <div className="flex-1">
                                <h3 className="font-bold text-lg">{item.name}</h3>
                                <p className="text-gray-600">${item.price.toLocaleString()} c/u</p>
                            </div>

                            <div className="flex items-center gap-2">
                                <button
                                    onClick={() => updateQuantity(item.id, -1)}
                                    className="bg-gray-300 text-black border border-gray-400 px-3 py-1 rounded hover:bg-gray-400 font-bold"
                                >
                                    -
                                </button>
                                <span className="px-4 font-medium text-black">{item.quantity}</span>
                                <button
                                    onClick={() => updateQuantity(item.id, 1)}
                                    className="bg-gray-300 text-black border border-gray-400 px-3 py-1 rounded hover:bg-gray-400 font-bold"
                                >
                                    +
                                </button>
                            </div>

                            <div className="text-right">
                                <p className="font-bold">${(item.price * item.quantity).toLocaleString()}</p>
                                <button
                                    onClick={() => removeItem(item.id)}
                                    className="text-red-500 text-sm hover:underline"
                                >
                                    Eliminar
                                </button>
                            </div>
                        </div>
                    ))}
                </div>

                <div className="bg-white rounded-lg shadow p-6 h-fit">
                    <h2 className="text-xl font-bold mb-4">Resumen</h2>

                    <div className="space-y-2 mb-4">
                        <div className="flex justify-between">
                            <span>Subtotal:</span>
                            <span>${subtotal.toLocaleString()}</span>
                        </div>
                        <div className="flex justify-between">
                            <span>IVA (19%):</span>
                            <span>${tax.toLocaleString()}</span>
                        </div>
                        <div className="flex justify-between font-bold text-lg border-t pt-2">
                            <span>Total:</span>
                            <span className="text-primary">${total.toLocaleString()}</span>
                        </div>
                    </div>

                    <button
                        onClick={checkout}
                        className="w-full bg-primary text-white py-3 rounded-lg hover:bg-amber-600 transition font-bold"
                    >
                        Finalizar Compra
                    </button>
                </div>
            </div>
        </div>
    )
}
