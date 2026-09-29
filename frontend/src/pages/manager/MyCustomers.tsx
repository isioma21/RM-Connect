import { useState } from 'react'
import { Box, Button, Card, Stack, Table, TableBody, TableCell, TableHead, TableRow, Typography } from '@mui/material'
import { relationshipsApi } from '../../api/relationships'
import type { Relationship } from '../../api/types'
import { ConfirmDialog } from '../../components/ConfirmDialog'
import { ErrorAlert } from '../../components/ErrorAlert'
import { formatDay } from '../../utils/format'

type Props = { customers: Relationship[]; onChanged: () => void }

export function MyCustomers({ customers, onChanged }: Props) {
  const [error, setError] = useState<unknown>(null)
  const [toEnd, setToEnd] = useState<Relationship | null>(null)

  async function end() {
    if (!toEnd) return
    setToEnd(null)
    try {
      await relationshipsApi.end(toEnd.id)
      onChanged()
    } catch (err) {
      setError(err)
    }
  }

  return (
    <Stack spacing={2}>
      <Typography variant="h2">My customers ({customers.length})</Typography>
      <ErrorAlert error={error} />
      <Card>
        {customers.length === 0 ? (
          <Typography sx={{ p: 3 }} color="text.secondary">No customers yet. Accept a request to add one.</Typography>
        ) : (
          <>
            {/* Phones: a simple list */}
            <Box sx={{ display: { md: 'none' } }}>
              {customers.map((relationship, index) => (
                <Stack key={relationship.id} spacing={0.5} sx={{ p: 2, borderTop: index ? 1 : 0, borderColor: 'divider', alignItems: 'flex-start' }}>
                  <Typography sx={{ fontWeight: 700 }}>{relationship.customer.firstName} {relationship.customer.lastName}</Typography>
                  <Typography variant="body2" color="text.secondary">{relationship.customer.email} · {relationship.customer.phone}</Typography>
                  <Typography variant="body2" color="text.secondary">
                    Customer since {relationship.respondedAt && formatDay(relationship.respondedAt)}
                  </Typography>
                  <Button color="error" onClick={() => setToEnd(relationship)} sx={{ px: 0 }}>End relationship</Button>
                </Stack>
              ))}
            </Box>
            <Table sx={{ display: { xs: 'none', md: 'table' } }}>
              <TableHead>
                <TableRow>
                  <TableCell>Customer</TableCell>
                  <TableCell>Email</TableCell>
                  <TableCell>Phone</TableCell>
                  <TableCell>Customer since</TableCell>
                  <TableCell />
                </TableRow>
              </TableHead>
              <TableBody>
                {customers.map((relationship) => (
                  <TableRow key={relationship.id}>
                    <TableCell sx={{ fontWeight: 700 }}>{relationship.customer.firstName} {relationship.customer.lastName}</TableCell>
                    <TableCell>{relationship.customer.email}</TableCell>
                    <TableCell>{relationship.customer.phone}</TableCell>
                    <TableCell>{relationship.respondedAt && formatDay(relationship.respondedAt)}</TableCell>
                    <TableCell align="right">
                      <Button color="error" onClick={() => setToEnd(relationship)}>End relationship</Button>
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </>
        )}
      </Card>
      <ConfirmDialog
        open={toEnd !== null}
        title="End relationship?"
        message={`Are you sure you want to end your relationship with ${toEnd?.customer.firstName}?`}
        confirmLabel="Yes, end relationship"
        onConfirm={end}
        onClose={() => setToEnd(null)}
      />
    </Stack>
  )
}
