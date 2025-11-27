'use client'

import { useEffect, useState } from 'react'

interface Product {
    id: number
    name: string
    description: string
    code: string
    price: number
    stock: number
    categoryName: string
}

export default function Home() {
    const [products, setProducts] = useState<Product[]>([])
    const [loading, setLoading] = useState(true)

    useEffect(() => {
        // Use relative URL so Next.js rewrites handle the proxy
        fetch('/api/Products')
            .then(res => res.json())
            .then(data => {
                setProducts(data)
                setLoading(false)
            })
            .catch(err => {
                console.error('Error fetching products:', err)
                setLoading(false)
            })
    }, [])

    const addToCart = (product: Product) => {
        const cart = JSON.parse(localStorage.getItem('cart') || '[]')
        const existing = cart.find((item: any) => item.id === product.id)

        if (existing) {
            existing.quantity += 1
        } else {
            cart.push({ ...product, quantity: 1 })
        }

        localStorage.setItem('cart', JSON.stringify(cart))
        alert(`${product.name} agregado al carrito`)
    }

    if (loading) {
        return (
            <div className="container mx-auto px-4 py-12 text-center">
                <p className="text-xl">Cargando productos...</p>
            </div>
        )
    }

    return (
        <div className="container mx-auto px-4 py-12">
            <h1 className="text-4xl font-bold mb-8 text-center">
                Catálogo de Productos
            </h1>

            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                {products.map(product => (
                    <div key={product.id} className="bg-white rounded-lg shadow-lg p-6 hover:shadow-xl transition">
                        <div className="mb-4">
                            <span className="text-sm text-gray-500">{product.categoryName}</span>
                            <h2 className="text-2xl font-bold mt-1">{product.name}</h2>
                            <p className="text-gray-600 mt-2">{product.description}</p>
                        </div>

                        <div className="flex justify-between items-center mb-4">
                            <span className="text-3xl font-bold text-primary">
                                ${product.price.toLocaleString()}
                            </span>
                            <span className="text-sm text-gray-500">
                                Stock: {product.stock}
                            </span>
                        </div>

                        <button
                            onClick={() => addToCart(product)}
                            disabled={product.stock === 0}
                            className="w-full bg-primary text-white py-2 px-4 rounded-lg hover:bg-amber-600 transition disabled:bg-gray-300 disabled:cursor-not-allowed"
                        >
                            {product.stock > 0 ? 'Agregar al Carrito' : 'Sin Stock'}
                        </button>
                    </div>
                ))}
            </div>
        </div>
    )
}
