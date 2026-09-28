export type Role = 'Customer' | 'RelationshipManager'

export type User = {
  id: string
  firstName: string
  lastName: string
  email: string
  phone: string | null
  branch: string | null
  role: Role
}

export type RegisterCustomerRequest = {
  firstName: string
  lastName: string
  email: string
  phone: string
  password: string
}

export type RegisterManagerRequest = {
  firstName: string
  lastName: string
  workEmail: string
  branch: string
  password: string
}
