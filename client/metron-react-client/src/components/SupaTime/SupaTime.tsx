import { useEffect, useState } from 'react'
import { createClient } from '@supabase/supabase-js'
import './SupaTime.css'

const supabaseUrl = import.meta.env.VITE_SUPABASE_URL
const supabaseKey = import.meta.env.VITE_SUPABASE_KEY
const supabaseEmail = import.meta.env.VITE_SUPABASE_USER_EMAIL
const supabasePassword = import.meta.env.VITE_SUPABASE_USER_PASSWORD
const supabase = createClient(supabaseUrl, supabaseKey)

const SupaTime = () => {
    const [records, setRecords] = useState<any[]>([]);

    useEffect(() => {

        async function retrieveRecords() {
            await supabase.auth.signInWithPassword({ email: supabaseEmail, password: supabasePassword })
            const { data } = await supabase.from('ingestion').select();
            if (data) {
                setRecords(data);
            }
        }

        retrieveRecords();
    }, [])

    return (
        <table>
            <thead>
                <tr>
                    <th>id</th>
                    <th>created_at</th>
                    <th>user_id</th>
                    <th>data_schema_name</th>
                    <th>data_schema_version</th>
                </tr>
            </thead>
            <tbody>
                {records.map((record) => (
                    <tr>
                        <td>{record.id}</td>
                        <td>{record.created_at}</td>
                        <td>{record.user_id}</td>
                        <td>{record.data_schema_name}</td>
                        <td>{record.data_schema_version}</td>
                    </tr>
                ))}
            </tbody>
        </table>
    )
}

export default SupaTime